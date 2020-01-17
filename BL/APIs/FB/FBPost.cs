using KoobecaFeedController.BL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBSynch.APIs.FB
{
    public class FBPosts
    {
        public Posts posts;

        public class Posts
        {
            public FBPost[] data;
        }
    }

    public class FBPost
    {
        //link,updated_time,full_picture,is_popular,shares,message,description,likes,comments
        public string source { get; set; }
        public string link { get; set; }
        public DateTime created_time { get; set; }
        public string full_picture { get; set; }
        public string name { get; set; }
        public string message { get; set; }
        public string description { get; set; }
        public string id { get; set; }
        public FBPrivacy privacy { get; set; }
        public FBAttachments attachments { get; set; }
        public Stream[] Photos { get; set; }

        public FBPostType Type = FBPostType.Link;
        public string source_type;
        
        public string Privacy
        {
            get
            {
                if (privacy == null) return "unknown";
                return privacy.value.ToLower().Replace("all_", "");
            }
        }

        public void SetType()
        {
            if(attachments != null && attachments.data.Any(a=>a.type == "photo" || a.type == "album"))
            {
                Type = FBPostType.Picture;
                return;
            }

            if(link == null)
            {
                if (string.IsNullOrEmpty(message))
                {
                    Type = FBPostType.Unknown;
                    Logger.Instance.Warn($"no link found for post {id} of {source}");
                }
                else
                {
                    Type = FBPostType.Simple;
                }
                return;
            }

            if (link.Contains("facebook.com"))
            {
                if (link.Contains("/videos/"))
                {
                    Type = FBPostType.Link; //currently fv video links are left as is
                }
                else if (!string.IsNullOrEmpty(full_picture))
                {
                    Type = FBPostType.Picture;
                }
                else
                {
                    Type = FBPostType.Simple;
                }
            }
            //else if(!string.IsNullOrEmpty(full_picture) && full_picture.Contains("/fbcdn."))
            //{
            //    Type = FBPostType.Picture;
            //}

        }

        public class FBPrivacy
        {
            public string value;
        }

        public class FBAttachment
        {
            public string type { get; set; }
            public string url { get; set; }
            public FBAttachments subattachments {get; set;}
            public Media media;

            public class Media
            {
                public Image image;
                

                public class Image
                {
                    public string src;
                }
            }
        }

        public class FBAttachments
        {
            public FBAttachment[] data { get; set; }
        }

        
    }

    public enum FBPostType
    {
        Simple,
        Picture,
        Link,
        Unknown
    }
}
