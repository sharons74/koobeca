
using FBSynch.APIs.FB;
using KoobecaFeedController.BL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace KoobecaSync.APIs.FB
{
    

    public class FacebookService 
    {
        private static string defaultToken = "EAAAAAYsX7TsBALOpEIIMWSDbIAfs9NkxRdhLDeRnS8EE8oUmQIn3CMCync754u11wlOmV7vsZAYoDfCwEHMbRnaZB3P1f7N9YDu6g5qtpLjXxf0RaLMmYnRe0VEVGVQ9LstqBVUByFCNGLZChaNW0MFyB0a50ZAG25rvucj6Ug852hesZBQWRLpSRwr7HMfVsCpIodZCUwGZAscv8UXgJHB";
       
        private static FacebookService _Default = new FacebookService(new FacebookClient(defaultToken), defaultToken);

        private readonly FacebookClient _facebookClient;
        private string _accessToken;

        public FacebookService(FacebookClient facebookClient,string accessToken)
        {
            _facebookClient = facebookClient;
            _accessToken = accessToken;
        }

        public async Task<Account> GetAccountAsync(string source)
        {  
            var result = await _facebookClient.GetAsync<Account>(
                source, "fields=id,name,email,first_name,last_name,age_range,birthday,gender,locale");
            
            return result;
        }

        public async Task<FBVideo> GetVideoAsync(string videoId)
        {
            var result = await _facebookClient.GetAsync<FBVideo>(
                videoId, "fields=source,title,description,picture,from");

            return result;
        }

        public async Task<FBPost[]> GetPostsAsync(string fbSource,string srcType,DateTime oldestTime)
        {
            var result = await _facebookClient.GetAsync<FBPosts>(
               fbSource, "fields=posts{link,created_time,full_picture,id,shares,name,message,description,likes,comments,privacy,attachments}");
            
            var posts = new List<FBPost>();
            try
            {
                foreach (var post in result.posts.data)
                {   
                    if (post.created_time > oldestTime)
                    {
                        post.source = fbSource;
                        post.source_type = srcType;
                        post.SetType();
                        if (post.Type != FBPostType.Unknown)
                            posts.Add(post);
                    }
                }
            }
            catch(Exception e)
            {
                Logger.Instance.Warn($"can't get posts of {fbSource} : {e.Message}");
            }
            return posts.ToArray();
        }

        public async Task<FBPost> GetSinglePostAsync(string postId)
        {
            return await _facebookClient.GetAsync<FBPost>(
               postId, "fields=link,created_time,full_picture,id,shares,name,message,description,likes,comments,privacy,attachments");

        }

        public async Task<FBPage> GetPageAsync(string fbSource)
        {
            var result = await _facebookClient.GetAsync<FBPage>(
                fbSource, "fields=id,category,name,link,page_token");
            return result; 
        }

        public static FacebookService Default {
            get
            {
                return _Default;
            }
        }

        public FacebookClient Client
        {
            get { return _facebookClient; }
        }
    }  
}