using FB2Koobeca.Entities;
using FBSynch.APIs.Koobeca;
using KoobecaFeedController.BL;
using KoobecaFeedController.BL.APIs.Entities;
using KoobecaFeedController.BL.APIs.Koobeca.Requests;
using KoobecaSync.APIs.Koobeca.Requests;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace KoobecaSync.APIs.Koobeca
{
    public class KoobecaService 
    {
        private readonly KoobecaClient _koobecaClient;
        private static KoobecaService _Default;

        public static KoobecaService Default {
            get
            {
                if(_Default == null)
                { 
                    _Default = new KoobecaService(new KoobecaClient());
                    var account = _Default.GetAccountAsync("pageadmin@koobeca.com", "123456").Result;
                }
                return _Default;
            }
        }

        public KoobecaService(KoobecaClient koobecaClient)
        {
            _koobecaClient = koobecaClient;
        }       

        public async Task<KBAccount> GetAccountAsync(string email, string password)
        {
            var result = await _koobecaClient.PostAsync<GetKBAccountResult>("login",null ,$"oauth_consumer_key={KoobecaSettings.ConsumerKey}&oauth_consumer_secret={KoobecaSettings.ConsumerSecret}&email={email}&password={password}&ip=127.0.0.1&subscriptionForm=");
            
            _koobecaClient.Token = result.body.oauth_token;
            _koobecaClient.Secret = result.body.oauth_secret;

            return result.body;
        }

        public async Task<PostResult> PostOnWallAsync(string privacy,int video_id,string message,string url,PostTarget target,int subjectId,Stream[] photos)
        {
            string type = string.IsNullOrEmpty(url) ? "" : "link";
            if(video_id != 0)
            {
                type = "video";
                url = "";
            }
            string subject_type = target == PostTarget.User ? "user" : "sitepage_page";

            var req = new KoobecaPostRequest()
            {
                body = message,
                type = type,
                video_id = video_id,
                uri = url,
                subject_type = subject_type,
                subject_id = subjectId,
                post_attach = 1,
                photos = photos,
                auth_view = privacy
            };

            var result = await PostKoobecaRequestAsync<PostResult>(req);
            
            if (result == null)
            {
                result = new PostResult() { error = true , message = "got null result" };
            }
            else
            {
                result.error = result.status_code != 200;
            }
            
            return result;
        }

        public async Task<VideoResult> PostVideoAsync(string videoType,string privacy, string title,string description,string url)
        {
            if (string.IsNullOrEmpty(title)) title = "---";
            if (string.IsNullOrEmpty(description)) description = "---";
            var req = new KoobecaCreateVideoRequest()
            {
                auth_view = privacy,
                title = title,
                description = description,
                url = url,
                type = videoType
            };

            var result = await PostKoobecaRequestAsync<VideoResult>(req);

            if (result == null)
            {
                result = new VideoResult() { error = true, message = "got null result" };
            }
            else
            {
                result.error = result.status_code != 200;
            }

            return result;
        }


        public int CreatePageAsync(string name, string url,string description,string category,Stream[] photos)
        {
            Logger.Instance.Debug($"creating koobeca page {name} ");
            try
            {  
                var req = new KoobecaCreatePageRequest
                {
                    body = name,
                    photos = photos,
                    sitepage = name,
                    title = name,
                    page_url = url,
                    location = "Earth",
                    package_id = 1,
                    category_id = 7,
                    auth_comment = "everyone",
                };

                var postTask = PostKoobecaRequestAsync<PageResult>(req);
                Task.WaitAll(postTask);

                var result = postTask.Result;
                
                if (result == null || result.status_code != 200)
                {
                    throw new System.Exception($"error_code:{result.status_code} message:{result.message}");
                }

                return result.body.page_id;
            }
            catch (System.Exception e)
            {
                Logger.Instance.Error($"Failed to create page for {name}:{e.Message}");
                return 0;
            }
            
            
        }

        public async Task<KBPage[]> GetPagesAsync()
        {
            int index = 1;
            bool doBrowse = true;
            List<KBPage> pages = new List<KBPage>();
            int pageCount = 0;

            while (doBrowse)
            {

                string args = $"oauth_consumer_key={KoobecaSettings.ConsumerKey}&oauth_consumer_secret={KoobecaSettings.ConsumerSecret}&page={index}&limit=200";// &_ANDROID_VERSION=3.1.7&language=en&restapilocation=";

                var result = await _koobecaClient.GetAsync<PostResult>("sitepages/browse", args);

                if (result == null || result.status_code != 200 || !(result.body.response is KBPage[]))
                {
                    return pages.ToArray();
                }
               
                foreach (var page in result.body.response as KBPage[])
                {
                    pages.Add(new KBPage
                    {
                        page_id = page.page_id,
                        page_url = page.page_url
                    });
                    pageCount++;
                    doBrowse = true; //browse more
                }
                index += 20; //get next 20
                if (pageCount < index - 1)
                    return pages.ToArray();
              
            }

            return pages.ToArray();
        }

        public enum PostTarget
        {
            User,
            Page
        }

        private async Task<T> PostKoobecaRequestAsync<T>(KoobecaApiRequest req)
        {
            string args = $"oauth_consumer_key={KoobecaSettings.ConsumerKey}&oauth_consumer_secret={KoobecaSettings.ConsumerSecret}&_ANDROID_VERSION=3.1.7&language=en&restapilocation=";

            return await _koobecaClient.PostReqAsync<T>(req.Endpoint, req, args);
        }
    }  
}