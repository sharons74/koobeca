using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using KoobecaFeedController.BL.Rendering;
using KoobecaFeedController.BL.Request;
using KoobecaFeedController.DAL.Adapters;
using KoobecaFeedController.DAL.Models;
using Newtonsoft.Json;

namespace KoobecaFeedController.BL.Handlers
{
    public class ViewHandler : ActivityHandler
    {
        private Dictionary<string, string> data = new Dictionary<string, string>();
        private bool internal_op;


        public ViewHandler(uint userId, RequestParams reqParams, Dictionary<string, object> additionalParams) : base(userId, reqParams)
        {
            ulong actionId = ulong.Parse((string)additionalParams["action_id"]);
            internal_op = (string)additionalParams["internal"] == "1";
            activity = ActivityCache.Instance.Get(actionId);
        }

        public override void Execute()
        {
            ActivityEntity subject = new ActivityEntity(activity, EntityType.Subject, 0);
            data["subject"] = subject.Label;
            data["subject_img"] = subject.Url;
            if (activity.Privacy != Privacy.Everyone)
            {
                data["type"] = "private";
            }
            else
            {
                
                data["body"] = HtmlHelper.WrapLinksWithATag(activity.RawActivity.BodyStr, out string url);
                data["url"] = url != null ? url : $"https://beta.koobeca.com/picHost.php?action_id={activity.ActivityId}";
                if (data["body"].Contains("href="))
                {
                    //get the url from there ?
                }
                if (activity.RawActivity.Attachments != null && activity.RawActivity.Attachments.Count > 0)
                {
                    //set attachment data
                    SetAttachmentData(activity.RawActivity.Attachments[0]);
                }

                if (!data.ContainsKey("img") || string.IsNullOrEmpty(data["img"]))
                {
                    data["img"] = subject.Url;
                    data["img_width"] = "720";
                    data["img_height"] = "540";
                }
                data["storage_img"] = data["img"];
                StoreImageFileLocally();
            }
            Response = JsonConvert.SerializeObject(data);
            Logger.Instance.Debug(Response);
        }

        private void StoreImageFileLocally()
        {
            if (internal_op) return; //we don't do it for internal calls from the smartphone

            if (data.ContainsKey("img"))
            {
                try
                {
                    Logger.Instance.Debug($"img file is {data["img"]}");
                    Uri uri = new Uri(data["img"]);
                    if (uri.IsAbsoluteUri)
                    {
                        string filename = GetFileName(data["img"]);
                        if (!File.Exists($"/home/koobadmin/koobse/shareFiles/{filename}"))
                        {
                            var webClient = new WebClient();
                            var bytes = webClient.DownloadData(data["img"]);
                            File.WriteAllBytes($"/home/koobadmin/koobse/shareFiles/{filename}", bytes);
                        }
                        if (File.Exists($"/home/koobadmin/koobse/shareFiles/{filename}"))
                        {
                            Logger.Instance.Debug($"/home/koobadmin/koobse/shareFiles/{filename} written successfuly.");
                            data["img"] = $"http://beta.koobeca.com/shareFiles/{filename}";
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Instance.Error("failed to save image file locally");
                    Logger.Instance.Debug(e.ToString());
                }
            }
        }

        private string GetFileName(string hrefLink)
        {
            Regex search = new Regex(@".+\/(.+\.jpg|.+\.png|.+\.bmp|.+\.gif)\??.*");
            var match = search.Match(hrefLink);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            return null;
            //string[] parts = hrefLink.Split('/');
            //string fileName = "";

            //if (parts.Length > 0)
            //    fileName = parts[parts.Length - 1];
            //else
            //    fileName = hrefLink;

            //return fileName;
        }

        private void SetAttachmentData(ActivityAttachment activityAttachment)
        {
            var iface = activityAttachment.GetActivityObject();
            data["type"] = activityAttachment.type;
            data["title"] = iface.GetObjectTitle();
            data["description"] = iface.GetObjectDesctiption();
            //data["url"] = HttpUtility.UrlEncode(coreLink.uri);
            if (iface.GetStorageFileId() > 0)
            {
                var storage = Storages.GetByFileId(iface.GetStorageFileId());
                var imageSize = JsonConvert.DeserializeObject<FeedImageSize>(storage.@params);
                data["img"] = ServiceUrlPrefixes.CouldFront + storage.storage_path;
                data["img_width"] = imageSize.width.ToString();
                data["img_height"] = imageSize.height.ToString();
            }

            if(iface is Video)
            {
                var video = iface as Video;
                data["video_type"] = video.type;
                if (video.type == "fb")
                {
                   Dictionary<string, string> code = JsonConvert.DeserializeObject<Dictionary<string, string>>(video.code);
                    data["img"] = code["thumb"];
                    data["img_width"] = "600";
                    data["img_height"] = "600";
                }
                //else
                //{
                    data["video_code"] = video.code;
                    data["url"] = video.GetVideoUrl(ServiceUrlPrefixes.CouldFront);
                //}
                //if (video.file_id > 0)
                //{
                //    var storage = Storages.GetByFileId(video.file_id);
                //    data["url"] = HttpUtility.UrlEncode(ServiceUrlPrefixes.CouldFront + storage.storage_path);
                //}
            }
            
            if(iface is CoreLink)
            {
                var link = iface as CoreLink;
                data["url"] = link.uri;
            }
        }
    }
}
