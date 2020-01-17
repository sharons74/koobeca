using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using KoobecaFeedController.BL.APIs;
using KoobecaFeedController.BL.Request;
using KoobecaFeedController.DAL.Adapters;
using KoobecaFeedController.DAL.Models;
using KoobecaSync.APIs.FB;
using KoobecaSync.APIs.Koobeca;
using Newtonsoft.Json;

namespace KoobecaFeedController.BL.Handlers
{
    public class PostHandler : UpdateHandler
    {
        private bool activityModified = false;

        public PostHandler(uint userId, RequestParams reqParams) : base(userId, reqParams)
        {
        }

        public override void Execute()
        {
           // if (HandlePromptCommands())
           // {
           //     return; //special prompt handling
           // }

            base.Execute();
            Logger.Instance.Debug($"action privacy is {activity.RawActivity.privacy} and given in parameter : {reqParams.auth_view}");
            if(string.IsNullOrEmpty(activity.RawActivity.privacy))
            {
                activity.RawActivity.privacy = reqParams.auth_view; //will fix this in fb posts
                activityModified = true;
            }
            HandleFBUserPosts();
            HandleFBLinks();
            HandleFBVideos();
            
            //check if the post contain interesting source like fb page
            CheckForSources();
            if (activityModified)
            {
                ActivityActions.Update(activity.RawActivity);
            }
            //return the feed
            Logger.Instance.Debug($"returning the feed");
            GetFeedHandler getFeedHandler = new GetFeedHandler(userId, reqParams);
            getFeedHandler.Execute();
            Response = getFeedHandler.Response;
        }

        private void HandleFBLinks()
        {
            try
            {
                Regex regex = new Regex(@".*\.facebook\.com/([0-9]+)/posts/([0-9]+)/?.*");
                //try body first
                var match = regex.Match(activity.RawActivity.BodyStr);
                if (match.Success && match.Groups.Count >= 3)
                {
                    var id = match.Groups[1].Value + "_" + match.Groups[2].Value;
                    var res = FacebookService.Default.GetSinglePostAsync(id).Result;
                    if (res.attachments != null && res.attachments.data.Length > 0 && !string.IsNullOrEmpty(res.attachments.data[0].url))
                    {
                        activity.RawActivity.BodyStr = res.attachments.data[0].url;
                        activityModified = true;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Warn("Failed to Handle FB Links:" + e.Message);
                Logger.Instance.Debug(e.ToString());
            }
            
        }

        private bool HandlePromptCommands()
        {
            if (userId != 3) return false;

            string cmd = activity.RawActivity.BodyStr.ToLower();
            if (!cmd.StartsWith("kb:")) return false;
            PromptCommand prompt = new PromptCommand(activity);
            prompt.Execute(cmd);

            return true;
        }

        private void HandleFBUserPosts()
        {
            if(activity.SubjectId == 339 && activity.SubjectType == "user" && activity.RawActivity.object_type == "user")
            {
                activity.RawActivity.subject_id = activity.RawActivity.object_id;
                var newType = "post_self_link";
                if (activity.RawActivity.Attachments != null && activity.RawActivity.Attachments.Count > 0)
                {
                    if (activity.RawActivity.Attachments[0].type == "album_photo")
                    {
                        newType = "post_self_photo";
                    }
                    else if (activity.RawActivity.Attachments[0].type == "video")
                    {
                        newType = "post_self_video";
                    }
                }
                activity.RawActivity.type = newType;
                activityModified = true;
                Logger.Instance.Debug($"Made activity {activity.ActivityId} belong to {activity.SubjectType}:{activity.SubjectId}");
            }
        }

        private void HandleFBVideos()
        {
            try
            {
                var videoId = GetFBVideoId(activity.RawActivity.BodyStr); //try the body first
                if (!string.IsNullOrEmpty(videoId))
                {
                    activity.RawActivity.body = null;
                }
                else if(activity.RawActivity.Attachments.Any(a=>a.type == "core_link"))
                {
                    //look inside the core link
                    var linkId = activity.RawActivity.Attachments.First(a => a.type == "core_link").id;
                    videoId = GetFBVideoId(CoreLinks.GetById(linkId).uri);
                }

                if (string.IsNullOrEmpty(videoId)) return;

                var videoData = GetVideoDataFromFB(videoId);

                //else

                //clean all existing attachments
                ActivityAttachments.DeleteForActivity(activity.ActivityId);
                ;
                //build the video attachment
                //var urls = new { video = iframely.url, thumb = iframely.links.thumbnail[0].href };
                Video newVideo = new Video()
                {
                    code = videoData["code"],
                    description = videoData["description"],
                    title = videoData["title"],
                    owner_id = (int)activity.OwnerId,
                    owner_type = activity.OwnerType,
                    type = "fb"
                };

                AddVideoAttachmentToActivity(newVideo,activity);
                activity.RawActivity.body = Encoding.UTF8.GetBytes(FilterUrlFromBody(activity.RawActivity.BodyStr));
            }
            catch
            {
                Logger.Instance.Debug($"Failed to attach FB video");
                return;
            }
        }

        private void AddVideoAttachmentToActivity(Video newVideo, Activity activity)
        {
            newVideo.video_id = (uint)Videos.Add(newVideo);
            var attachment = new ActivityAttachment()
            {
                action_id = (uint)activity.ActivityId,
                id = newVideo.video_id,
                type = "video",
                mode = true
            };
            attachment.attachment_id = (uint)ActivityAttachments.Add(attachment);

            activity.RawActivity.Attachments = new List<ActivityAttachment>();
            activity.RawActivity.Attachments.Add(attachment);
            activity.RawActivity.attachment_count = 1;
            activity.RawActivity.type = "post_self_video";
            activityModified = true;
            Logger.Instance.Debug($"Attached FB video to {activity.ActivityId}");
        }

        private string GetFBVideoId(string input)
        {
            Regex regex = new Regex(@".*\.facebook\.com/.+/videos/([0-9]+)/?.*");
            //try body first
            var match = regex.Match(input);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return null;
        }

        

        private string FilterUrlFromBody(string body)
        {

            var url = HtmlHelper.FindUrl(body);
            if (!string.IsNullOrEmpty(url)){
                body = body.Replace(url, "");
            }
            return body;
        }

        private Dictionary<string, string> GetVideoDataFromFB(string videoId)
        {
            Dictionary<string, string> videoData = new Dictionary<string, string>();

            FBVideo fbVideo = FacebookService.Default.GetVideoAsync(videoId).Result;

            var urls = new { video = fbVideo.source, thumb = fbVideo.picture };

            videoData["code"] = JsonConvert.SerializeObject(urls);
            videoData["description"] = HttpUtility.HtmlEncode(fbVideo.description);// Convert.ToBase64String(Encoding.UTF8.GetBytes(fbVideo.description));
            videoData["title"] = $"{fbVideo.from.name}|{fbVideo.title}";
            return videoData;
        }

        private Dictionary<string, string> GetVideoDataWithIFramely(string url)
        {
            Dictionary<string, string> videoData = new Dictionary<string, string>();
            var json = IFramelyUtil.Get(url);
            if (string.IsNullOrEmpty(json)) return null;

            var iframely = JsonConvert.DeserializeObject<Framely>(json);
            if (iframely.links.player.Length == 0) return null;
            var urls = new { video = iframely.url, thumb = iframely.links.thumbnail[0].href , html = iframely.links.player[0].html };

            videoData["code"] = JsonConvert.SerializeObject(urls);
            videoData["description"] = iframely.meta.description;
            videoData["title"] = iframely.meta.title;

            return videoData;
        }

        private void CheckForSources()
        {
            if (userId == 339) return;//pageadmin 

            string srcId = FindFBSource(activity.RawActivity.BodyStr);
            if(!string.IsNullOrEmpty(srcId))
            {
                Logger.Instance.Debug($"found fb source {srcId} in post.looking for page");

                PageSource record = PageSources.GetBySourceId(srcId, "fb_page");
                if (record == null)
                {
                    //create a page and assign this action to it
                    Task.Run(() => CreatePageAndAssignActivity(srcId, activity.ActivityId));
                }
                else
                {
                    //like or increase like to this page
                    LikeSourceIfPossible(record.page_id, "sitepage_page");
                }
            }
        }

        private string FindFBSource(string body)
        {
            var fbMatch1 = "https://www.facebook.com/";
            var fbMatch2 = "m.facebook.com/story.php?";
            if (body.Contains(fbMatch1))
            {
                int index = body.IndexOf(fbMatch1) + fbMatch1.Length;
                int end = body.IndexOf("/", index);
                return body.Substring(index, end - index);
                //Logger.Instance.Debug($"found fb source {src} in post.looking for page");
            }
            else if (body.Contains(fbMatch2))
            {
                int index = body.IndexOf(fbMatch2) + fbMatch2.Length;
                var subHtml = body.Substring(index);
                int endIndex = subHtml.IndexOf(" ", index);
                if(endIndex > 0)
                {
                    subHtml = subHtml.Substring(0, endIndex);
                }
                Uri url = new Uri("http://www.dummyurl.com?" + subHtml);
                var id = HttpUtility.ParseQueryString(url.Query).Get("id");
                return id;
            }

            return null;
        }

        private void CreatePageAndAssignActivity(string fbSrcId, ulong actionId)
        {
            try
            {
                Logger.Instance.Debug($"Creating page for {fbSrcId}");

                var fbPage = FacebookService.Default.GetPageAsync(fbSrcId).Result;
                //get photo stream
                var photo = FacebookService.Default.Client.GetStreamContentAsync($"{fbSrcId}/picture", "type=large").Result;
                // Create koobeca page
                var pageId = KoobecaService.Default.CreatePageAsync(fbPage.name, fbPage.link, fbPage.description, fbPage.categoty,new System.IO.Stream[1] { photo });
                if (pageId > 0)
                {
                    PageSources.Add((ulong)pageId, fbSrcId, fbPage.name, "fb_page");
                    LikeSourceIfPossible((uint)pageId, "sitepage_page");
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Error($"{e.ToString()}");
            }
        }
    }
}
