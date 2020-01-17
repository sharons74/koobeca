using FB2Koobeca;
using FB2Koobeca.Entities;
using FB2Koobeca.Utils;
using FBSynch.APIs.FB;
using KoobecaFeedController.DAL.Adapters;
using KoobecaSync.APIs.FB;
using KoobecaSync.APIs.Koobeca;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FBSynch
{
    public class Controller
    {
        public event Action<string, double> Published;

        private KoobecaClient _KoobecaClient = null;
        private KoobecaService _KoobecaService = null;
        private FacebookClient _FacebookClient = null;
        private FacebookService _FacebookService = null;
        private FacebookPostFetcher _Fetcher = null;
        private KoobecaPublisher _Publisher = null;
        private Queue<FBPost> PostQueue = new Queue<FBPost>();
        private bool _IsInited = false;

        public void Start(string koobecaUser,string koobecaPassword,string fb_token)
        {
            try
            {
                int minutesBack = 1;// 1500;
                InitializeServices(koobecaUser,koobecaPassword,fb_token);
                
                
                Queue<FBPost> queue = new Queue<FBPost>();
                List<Task> tasks = new List<Task>();
                //start fetching posts and publishing them
                _Fetcher = new FacebookPostFetcher(fb_token, queue);
                _Fetcher.Start(20,minutesBack);
                _Publisher = new KoobecaPublisher(_KoobecaService, queue);
                _Publisher.Published += _Publisher_Published;
                _Publisher.Start(koobecaUser, koobecaPassword);
            }
            catch(Exception e)
            {
                Logger.Instance.Error(e.Message);
                StopAll();
            }
        }

        private void _Publisher_Published(string url, double timeSpent)
        {
            Published?.Invoke(url, timeSpent);
        }

        public void StopAll()
        {
            _Publisher?.Stop();
            _Fetcher?.Stop();
        }

        public void InitializeServices(string koobecaUser, string koobecaPassword, string fb_token)
        {
            if (_IsInited) return;

            LoginToKoobeca(koobecaUser, koobecaPassword);
            CreateFacebookService(fb_token);
            _IsInited = true;

        }

        public FBPost CurrentlyPublished
        {
            get
            {
                return _Publisher.CurrentlyPublished;
            }
        }

        public int PublishCount
        {
            get
            {
                return _Publisher.PublishCount;
            }
        }

        public int PublishErrorCount
        {
            get
            {
                return _Publisher.PublishErrorCount;
            }
        }

        private void CreateFacebookService(string fb_token)
        {
            _FacebookClient = new FacebookClient(fb_token);
            _FacebookService = new FacebookService(_FacebookClient,fb_token);
        }
       
        private void LoginToKoobeca(string user,string password)
        {
            if (string.IsNullOrEmpty(user))
                throw new Exception("Koobeca user is missing");
            if (string.IsNullOrEmpty(password))
                throw new Exception("Koobeca password is missing");

            Logger.Instance.Info("Login in to koobeca");
            //login
            _KoobecaClient = new KoobecaClient();
            _KoobecaService = new KoobecaService(_KoobecaClient);
            var getAccountTask = _KoobecaService.GetAccountAsync(user, password);
            Task.WaitAll(getAccountTask);
            var account = getAccountTask.Result;
            if (string.IsNullOrEmpty(account.oauth_token))
                throw new Exception("Failed to login");
        }

        private void LoadKoobecaPages()
        {
            //Logger.Instance.Info("Loading koobeca page list");
            //var getPagesTask = _KoobecaService.GetPagesAsync();
            //Task.WaitAll(getPagesTask);
            //var pages = getPagesTask.Result;
            //if (pages.Length == 0)
            //    throw new Exception("Failed to load page list of koobeca");

            //foreach (var page in pages)
            //{
            //    _PageIDs[page.Url.ToLower()] = page.Id;
            //}

            //List<string> lines = new List<string>();
            //foreach(var pair in _PageIDs)
            //{
            //    lines.Add($"{pair.Key},{pair.Value}");
            //}

            //File.WriteAllLines("existingPages.txt", lines.ToArray());
            
        }

    }
}
