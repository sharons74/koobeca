using FB2Koobeca.Entities;
using FB2Koobeca.Utils;
using FBSynch;
using FBSynch.APIs.FB;
using FBSynch.APIs.Koobeca.Requests;
using KoobecaFeedController.BL.APIs.Entities;
using KoobecaFeedController.DAL.Adapters;
using KoobecaFeedController.DAL.Models;
using KoobecaSync.APIs.FB;
using KoobecaSync.APIs.Koobeca;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FB2Koobeca
{
    public class KoobecaPublisher
    {

        public event Action<string,double> Published ;

        
        //private string _PublishedPath;
        //private string _FailedPath;
        private bool IsActive { get; set; } = true;
        private KoobecaService _KoobecaService = null;
        private Queue<FBPost> _PostQueue;
        private Dictionary<string, PageSource> _PageSourceCache = new Dictionary<string, PageSource>();

        public bool ActualPublishing { get; set; } = true;
        public int PublishCount { get; private set; }
        public int PublishErrorCount { get; private set; }
        public FBPost CurrentlyPublished { get; private set; }

        

        public KoobecaPublisher(KoobecaService koobecaService,Queue<FBPost> queue){

            _KoobecaService = koobecaService;
            //_KoobecaPageIDs = koobecaPageIDs;
            _PostQueue = queue;
        }

        


        public Task Start(string user,string password)
        {
            Logger.Instance.Info("Start Publishing");
            return Task.Run(()=>DoPosting(user,password));
        }

        public void Stop()
        {
            Logger.Instance.Info("Stop Publishing");
            IsActive = false;
        }


        public void DoPosting(string user,string password)
        {
            //login
            var koobecaClient = new KoobecaClient();
            var koobecaService = new KoobecaService(koobecaClient);
            var getAccountTask = koobecaService.GetAccountAsync(user, password);
            Task.WaitAll(getAccountTask);
            var account = getAccountTask.Result;
            if (string.IsNullOrEmpty(account.oauth_token))
            {
                Logger.Instance.Error("Failed to login to koobeca");
                return;
            }

            //load the cache
            PageSources.GetByType("fb_page").Select(p => _PageSourceCache[p.source_id] = p).ToArray();
            Logger.Instance.Debug($"page source cache loaded with {_PageSourceCache.Count} items.");
           
            //take from the queue
            while (IsActive)
            {
                
                if (_PostQueue.Count > 0)
                {
                    var item = _PostQueue.Dequeue();

                    if (item.Type == FBPostType.Picture)
                    {
                        try
                        {
                            //make the picture include the bigger text
                            StringBuilder bigMsg = new StringBuilder();
                            bigMsg.AppendLine(item.message);
                            bigMsg.AppendLine("------------------------");
                            bigMsg.AppendLine(item.name);
                            bigMsg.AppendLine(item.description);
                            item.message = bigMsg.ToString();

                            //get all puctures
                            List<string> picUrls = new List<string>();
                            picUrls.AddRange(item.attachments.data.Where(a => a.type == "photo").Select(a => a.media.image.src).ToList());
                            picUrls.AddRange(item.attachments.data.Where(a => a.type == "album").SelectMany(a => a.subattachments.data.Where(sa => sa.type == "photo").Select(sa => sa.media.image.src)).ToList());
                            var webClient = new WebClient();

                            item.Photos = new Stream[picUrls.Count];
                            for (int i = 0; i < picUrls.Count; i++)
                            {
                                var url = picUrls[i];
                                byte[] imageBytes = webClient.DownloadData(url);
                                item.Photos[i] = new MemoryStream(imageBytes);
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Instance.Error($"failed to get photo stream from {item.full_picture} , {e.Message}");
                        }
                    }
                    Logger.Instance.Info($"Got post from FB source:{item.source} of type {item.Type} , looking for target in Koobeca");
                    double timeSpent;
                    PublishPost(_KoobecaService,item,out timeSpent);
                    Published?.Invoke(item.source,timeSpent/1000); // throw event to listeners
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }

            Logger.Instance.Info($"Publishing ended.");
            
        }

        private bool PublishPost(KoobecaService service,FBPost item,out double timeSpanMiliSec)
        {
            DateTime start = DateTime.Now;
            CurrentlyPublished = item;
            try
            {
                //if (string.IsNullOrEmpty(item.Link))
                //    item.Link = item.URL;
                Logger.Instance.Info($"Publishing {item.source_type} {item.source} : {item.message} : link {item.link}");

                if (item.Photos == null)
                {
                    var effectiveLink = UrlUnshortener.GetRealUrl(item.link);
                    if (effectiveLink != item.link)
                    {
                        Logger.Instance.Info($"effective link is: {effectiveLink}");
                        item.link = effectiveLink;
                    }
                }
                else
                {
                    //no link in case of photo
                    item.link = string.Empty;
                }

                if (!_PageSourceCache.TryGetValue(item.source, out PageSource source))
                {
                    source = PageSources.GetBySourceId(item.source, item.source_type);
                    if (source == null)
                    {
                        throw new Exception($"{item.source}:{item.link} not found in page ids");
                    }
                    else
                    {
                        //add it to the cache
                        _PageSourceCache[item.source] = source;
                    }
                }


                ulong pageId = source.page_id; //target id (legacy is page)
                if (ActualPublishing)
                {
                    KoobecaService.PostTarget targetType = source.source_type == "fb_page" ? KoobecaService.PostTarget.Page : KoobecaService.PostTarget.User;
                    int videoId = 0;
                    if (item.link.Contains(".youtube.com"))
                    {
                        //post video
                        var vidReult = service.PostVideoAsync("1", "everyone",item.name, item.description, item.link).Result;
                        if (!vidReult.error)
                        {
                            videoId = vidReult.body.response.video_id;
                        }
                    }

                    var result = service.PostOnWallAsync(item.Privacy,videoId,item.message,item.link,targetType,(int)pageId,item.Photos).Result;
                    if (result.error)
                    {
                        Logger.Instance.Error(result.ToString());
                    }
                }
                else
                {
                    Thread.Sleep(1000); // just sleep second
                }

                timeSpanMiliSec = (DateTime.Now - start).TotalMilliseconds;
                Logger.Instance.Info($"Time Spent in {timeSpanMiliSec} ms");

                //File.AppendAllText(_PublishedPath,item.ID + "\n");
                PublishCount++;
                return true; 
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e.ToString());
                PublishErrorCount++;
                timeSpanMiliSec = (DateTime.Now - start).TotalMilliseconds;
                return false;
            }
            finally
            {
                CurrentlyPublished = null;
            }
           
        }

        
    }
}
