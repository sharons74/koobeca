using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KoobecaFeedController.BL.Request
{
    public class RequestParams
    {
        public uint viewerId;
        public int rewrite;
        public string oauth_token;
        public string oauth_consumer_key;
        public string oauth_consumer_secret;
        public string oauth_secret;
        public string restapilocation;
        public ulong device_id;
        //\"_ANDROID_VERSION\":\"3.2.5\",
        public string language;
        public ulong id;
        public ulong action_id;
        public string body;
        public int send_notification;

        public uint subject_id;
        public string subject_type;
        public uint user_id;
        public string reaction;
        public ulong comment_id;
        public string attachment_type;
        public string attachment_id;
        public string viewAllComments;


        //feed specific
        public int minid;
        public int maxid;
        public int limit = 50;
        //public bool feed_filter;
        public string filter_type = "All";
        //public bool feed_count_with_content;
        //public int list_id;
        //public bool subject_info;
        //public bool object_info;

        public string _IOS_VERSION;
        public int ios;
        public string auth_view;
        //public int hide_report;
        public string composer;
        //\"getDeviceId\":\"869780032931062\",
        //\"getDeviceType\":\"all\",
        //\"viewSuffix\":\"phtml\"}"}'
        public string DisplayName;
        
        public ComposerData GetComposer()
        {
            if (string.IsNullOrEmpty(composer)) return null;

            return JsonConvert.DeserializeObject<ComposerData>(composer);
        }
    }

    public class ComposerData
    {
        public string tag;

        public List<ulong> GetTaggedUsers()
        {
            List<ulong> res = new List<ulong>();
            if (!string.IsNullOrEmpty(tag))
            {
                //split all by users
                foreach(var user in tag.Split('&').Where(u=>!string.IsNullOrEmpty(u)))
                {
                    var parts = user.Split('=');
                    if(parts.Length == 2)
                    {
                        if (parts[0].StartsWith("user_") && ulong.TryParse(parts[0].Replace("user_",""),out ulong userId))
                        {
                            if (!res.Contains(userId)) res.Add(userId);
                        }
                    }
                }
            }

            return res;
        }
    }
}
