
using FB2Koobeca.Utils;
using FBSynch.APIs.FB;
using KoobecaFeedController.DAL.Adapters;
using KoobecaFeedController.DAL.Models;
using KoobecaSync.APIs.FB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FBSynch
{
    public class FacebookPostFetcher
    {
        private FacebookService _Service = null;
        private DateTime _LastFetchTime;
        private Queue<FBPost> _PostQueue = null;
        private bool IsActive { get; set; } = true;

        public FacebookPostFetcher(string token, Queue<FBPost> queue)
        {
            _PostQueue = queue;
            _Service = new FacebookService(new FacebookClient(token), token);
        }

        public Task Start(int checkingIntervalInMinutes = 1,int startBackFromMinutes = 1)
        {
            Logger.Instance.Info("Start Fetching from Facebook");
            _LastFetchTime = DateTime.Now.AddMinutes(-startBackFromMinutes);
            IsActive = true;
            return Task.Run(() => DoFetching(checkingIntervalInMinutes));
        }

        public void Stop()
        {
            IsActive = false;
        }

        public void DoFetching(int checkingIntervalInMinutes = 1)
        {
            //load sourcelist
            //PageSources.Add(390, "1154233056", "Tal Gilad", "fb_user");

            while (IsActive)
            {
                //go through sources
                foreach (var source in PageSources.GetByType("fb_page").Select(s => s.source_id).ToList())
                {
                    FetchPosts(source,"fb_page");
                }
                foreach (var source in PageSources.GetByType("fb_user").Select(s => s.source_id).ToList())
                {
                    FetchPosts(source, "fb_user");
                }

                //update last fetch time
                _LastFetchTime = DateTime.Now;
                if (!IsActive) return;
                //go to sleep
                int sleepSeconds = checkingIntervalInMinutes * 60;
                while (sleepSeconds-- > 0 && IsActive)
                    Thread.Sleep(1000);
            }

            Logger.Instance.Info("Fetching process ended !");

        }
        

        private void FetchPosts(string source,string srcType) {

            try
            {
                var posts = _Service.GetPostsAsync(source,srcType,_LastFetchTime).Result.Take(3).ToArray();//take max 2 post each interval

                if(posts.Length > 0)
                 Logger.Instance.Info($"Adding {posts.Length} more posts from {source} to queue");

                foreach (var post in posts)
                {
                    if (!string.IsNullOrEmpty(post.link) && post.link.Contains(".koobeca.com/")) continue; //we skip posts that where posted from koobeca
                    if (post.Privacy != "everyone" && post.Privacy != "friends")
                    {
                        continue;
                    }
                    //put them in Queue
                    _PostQueue.Enqueue(post);
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Error($"Failed to fetch posts of {source}:{e.ToString()}");
            }
            
        }

        
    }

    
}
