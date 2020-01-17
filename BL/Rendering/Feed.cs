using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using KoobecaFeedController.DAL.Adapters;
using KoobecaFeedController.DAL.Models;
using Newtonsoft.Json;

// ReSharper disable InconsistentNaming

namespace KoobecaFeedController.BL.Rendering {
    public class Feed {
        

        public uint action_id;
        public string type;
        public string subject_type;
        public uint subject_id;
        public string object_type;
        public uint object_id;
        public string body;
        public object @params;
        public string date;
        public string modified_date;
        public uint attachment_count;
        public uint comment_count;
        public uint like_count;
        public string privacy;
        public uint likeable;
        public uint commentable;
        public uint shareable;
        public string user_agent;
        public string publish_date;
        public string privacy_icon;
        public object userTag;
        public FeedWordStile[] wordStyle;
        public object decoration;
        public uint feed_type;
        public ulong time_value;
        public SubjectInformation subjectIformation;
        public object @object;
        public string feed_icon;
        public string attachment_content_type;
        public FeedAttachment[] attachment;
        public uint photo_attachment_count;
        public string feed_title;
        public string action_type_body;
        public FeedActionTypeBodyParam[] action_type_body_params;
        public uint isRequired;

        //public uint feed_type;
        private readonly Activity _activity;
        private readonly ActivityAction _rawActivity;

        private ActivityEntity _Subject;
        private ActivityEntity _Owner;
        private ActivityEntity _Object;
        private ActivityEntity _Item;
        private ulong _ViewerId;
        private bool _isIOS;

        public Feed(Activity activity, ulong forSource, string sourceType,bool isIOS) {

            _activity = activity;
            _rawActivity = _activity.RawActivity;
            _ViewerId = forSource;
            _isIOS = isIOS;
            if (activity.IsLikeCommentOrReact)
            {
                attachment = GetAttachments(_activity, forSource, sourceType);
                FeedAttachmentActivity innerFeed = attachment[0] as FeedAttachmentActivity;
                CopyValsFrom(innerFeed.shared_post_data[0].feed);
                SetCommonFields();
                privacy_icon = null;
            }
            else //primary or share
            {
                action_id = _rawActivity.action_id;
                //object_type = _rawActivity.object_type;
                //object_id = _rawActivity.object_id;
                attachment = GetAttachments(_activity, forSource, sourceType);
                SetCommonFields();

                privacy = _activity.Privacy.ToString().ToLower();
                var authorization = _activity.GetAuthorization(forSource, sourceType);
                commentable = (uint)(((authorization & (ushort)AuthorizationFlags.Comment) > 0) & _activity.RawActivity.commentable ? 1 : 0); //(uint)(CanReact("comment",forSource,sourceType)?1:0);
                likeable = (uint)((authorization & (ushort)AuthorizationFlags.Like) > 0 ? 1 : 0);  //(uint)(CanReact("like", forSource, sourceType) ? 1 : 0);
                shareable = (uint)(((authorization & (ushort)AuthorizationFlags.Share) > 0) && _activity.RawActivity.shareable ? 1 : 0);  //(uint)(CanShare() ? 1 : 0);
                
                
                comment_count = _rawActivity.comment_count;
                like_count = GetLikeCount();
            }
            
        }

       

        private void SetCommonFields()
        {
           
            type = _rawActivity.type;
            subject_type = _activity.EffectiveSubjetType;
            subject_id = (uint)_activity.EffectiveSubjectId;
            object_type = _activity.ObjectType;
            object_id = _activity.ObjectId;
            body = GetBody();
            @params = !string.IsNullOrEmpty(_rawActivity.@params) ? JsonConvert.DeserializeObject(_rawActivity.@params) : "";
            date = _rawActivity.date.ToString("yyyy-MM-dd HH:mm:ss");
            modified_date = _rawActivity.modified_date.ToString("yyyy-MM-dd HH:mm:ss");
            action_type_body = GetActionTypeBody();// + $"\r\n{activity.ActivityId}";
            action_type_body_params = GetActionTypeBodyParams();
            feed_title = GetFeedTitle();// + $"\r\n{activity.ActivityId}";
            action_type_body = action_type_body.Replace("\r\n", " ");
            subjectIformation = new SubjectInformation { image = Subject.Url };
            feed_icon = GetFeedIcon();
            publish_date = _rawActivity.publish_date?.ToString("yyyy-MM-dd HH:mm:ss");
            privacy_icon = _activity.Privacy.ToString().ToLower(); //TODO check
            userTag = _rawActivity.GetSingleParam("tags") ?? new string[0];
            wordStyle = GetWordStyling();
            decoration = GetFeedDecorationSetting();
            feed_type = GetFeedType();
            time_value = (ulong)_rawActivity.date.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            user_agent = _rawActivity.user_agent;
            @object = GetObject();
            attachment_content_type = _rawActivity.Attachments.Count > 0 ? _rawActivity.Attachments[0].type : null;
            attachment_count = attachment == null ? 0 : (uint)attachment.Length;
            photo_attachment_count = (uint)(attachment?.Count(a => a?.HasPhoto ?? false) ?? 0);
        }

        private string GetBody()
        {
            string body = "";
            if (_activity.IsLikeOrReact)
            {
                // body remains empty
            }
            else if (attachment != null && attachment.Length > 0 && attachment[0] is FeedAttachmentCore)
            {
                if(((FeedAttachmentCore)attachment[0]).body == _rawActivity.BodyStr)
                {
                    //body remains empty
                }
                else
                {
                    body = $"{HttpUtility.HtmlDecode(_rawActivity.BodyStr)}";
                }
            }
            else
            {
                body = $"{HttpUtility.HtmlDecode(_rawActivity.BodyStr)}";
            }

            if(_ViewerId == 3)
            {
                body += $" {_activity.ActivityId}";
            }

            return body;
        }

        private void CopyValsFrom(Feed otherFeed)
        {
            action_id = otherFeed.action_id;
            
            modified_date = otherFeed.modified_date;
            comment_count = otherFeed.comment_count;
            like_count = otherFeed.like_count;
            privacy = otherFeed.privacy;
            likeable = otherFeed.likeable;
            commentable = otherFeed.commentable;
            shareable = otherFeed.shareable;
            publish_date = otherFeed.publish_date;
            isRequired = otherFeed.isRequired;
        }

        private uint GetLikeCount() {
            if (_activity.InnerActivity != null && _activity.InnerActivityId != _activity.ActivityId)
                return _activity.InnerActivity.RawActivity.like_count;
            return _rawActivity.like_count; //TODO , need to see if it is true for album photos
        }

        private object GetObject() {
            return new {
                //user_id = 3,
                //username = (string)null,
                //displayname = "Sharon Salmon",
                //photo_id = 6388,
                //status = "me only",
                //status_date = "2018-12-20 08:33:52",
                //locale = "en",
                //language = "en",
                //timezone = "Europe/Moscow",
                //search = 1,
                //show_profileviewers = 1,
                //level_id = 2,
                //invites_used = 2,
                //extra_invites = 0,

                //enabled = 1,
                //verified = 1,
                //approved = 1,

                //creation_date = "2018-06-27 08:08:24",
                //modified_date = "2018-12-20 13:21:16",
                //lastlogin_date = "2018-12-20 08:31:52",
                //update_date = (string)null,
                //member_count = 85,
                //view_count = 865,
                //comment_count = 0,
                //like_count = 0,
                //coverphoto = 4923,
                //coverphotoparams = "{\"top\":\"-399\",\"left\":\"0\"}",
                //view_privacy = "everyone",
                //seao_locationid = 0,
                //location = "",
                //name = "user",

                //image = "https://d18js22wsvyojh.cloudfront.net/public/user/0d/19/5ee203e9151c3d0da899441d0f4b2c2e.jpg",
                //image_normal = "https://d18js22wsvyojh.cloudfront.net/public/user/0f/19/a741cad67833ba7346fa710fdf9d4a78.jpg",
                //image_profile = "https://d18js22wsvyojh.cloudfront.net/public/user/0e/19/9d3b423d15724d0b9fc09aebafc559b1.jpg",
                //image_icon = "https://d18js22wsvyojh.cloudfront.net/public/user/10/19/424a492d509dbe325d11c6f60c65f469.jpg",

                url = $"https://beta.koobeca.com/profile/{_activity.ObjectId}",
                content_url = $"https://beta.koobeca.com/profile/{_activity.ObjectId}",
                owner_url = $"/profile/{_activity.ObjectId}"

                //owner_title = "Sharon Salmon"
            };
        }

        private string GetFeedIcon() {
            return Subject.Url;
        }

        public void SetActionId(uint action_id = 0)
        {
            if(action_id == 0)
            {
                if (!_activity.IsShare) return; //we only do this trick to share because we need to like the share and not the inner activity
                action_id = this.action_id;
            }

            this.action_id = action_id;
            if (attachment != null)
            {
                foreach (var att in this.attachment)
                {
                    if(att is FeedAttachmentActivity)
                    {

                       foreach(var fw in ((FeedAttachmentActivity)att).shared_post_data)
                        {
                            fw.feed.SetActionId(action_id);
                        }
                    }
                }
            }
        }

        private string GetFeedTitle() {
            var title = action_type_body;

            foreach(var p in action_type_body_params)
            {
                if (title.Contains(p.search))
                    title = title.Replace(p.search, p.label);
            }
            return title;
        }


        [JsonIgnore]
        public string ActedOn => VerbsMap.PastVerb(_activity.RawActivity.type);

        [JsonIgnore]
        public ActivityEntity Subject {
            get {
                if (_Subject == null) _Subject = new ActivityEntity(_activity, EntityType.Subject,_ViewerId);
                return _Subject;
            }
        }

        [JsonIgnore]
        public ActivityEntity Owner {
            get {
                if (_Owner == null) _Owner = new ActivityEntity(_activity, EntityType.Owner,_ViewerId);
                return _Owner;
            }
        }


        [JsonIgnore]
        public ActivityEntity Object {
            get {
                if (_Object == null) _Object = new ActivityEntity(_activity, EntityType.Object,_ViewerId);
                return _Object;
            }
        }


        [JsonIgnore]
        public List<string> TagList {
            get {
                var list = HashTagFinder.FindTags(body);
                //no need list.AddRange(HashTagFinder.FindTags(feed_title));
                if (attachment != null)
                    foreach (var attach in attachment)
                        if (attach != null)
                            list?.AddRange(attach.TagList);
                return list.GroupBy(test => test).Select(grp => grp.First()).ToList();
            }
        }

        private FeedActionTypeBodyParam[] GetActionTypeBodyParams() {
            var list = new List<FeedActionTypeBodyParam>();

            if (action_type_body.Contains("{item:$subject}"))
                list.Add(new FeedActionTypeBodyParamExt
                    {search = "{item:$subject}", label = Subject.Label, type = Subject.Type, id = Subject.Id});

            if (action_type_body.Contains("{item:$object}"))
                list.Add(new FeedActionTypeBodyParamExt
                    {search = "{item:$object}", label = Object.Label, type = Object.Type, id = Object.Id});

            if (action_type_body.Contains("{item:$owner}"))
                list.Add(new FeedActionTypeBodyParamExt
                    {search = "{item:$owner}", label = Owner.Label, type = Owner.Type, id = Owner.Id});
            var type = $"{{item:$object:{_activity.TemplateObjectType}}}";
            if (action_type_body.Contains(type))
                list.Add(new FeedActionTypeBodyParamExt
                    {search = type, label = _activity.TemplateObjectType, type = _activity.ObjectType, id = _activity.ObjectId});

            if (action_type_body.Contains("{body:$body}"))
                list.Add(new FeedActionTypeBodyParam
                    {search = "{body:$body}", label = body});

            return list.ToArray();
        }

        private string GetActionTypeBody() {
            var actionType = ActionTypes.Get(_activity.RawActivity.type);
            var body = ActionTypeTransformer.Transform(actionType?.body??"");
            if(!_activity.IsPrimary && _activity.InnerActivity != null && _activity.InnerActivity.EffectiveSubjetType == "sitepage_page")
            {
                //then we reffer to the page and not to the owner
                body = body.Replace("item:$owner", "item:$object");
            }
            if(body.Contains("{item:$object}'s") && Object.IsViewer)
            {
                body = body.Replace("{item:$object}'s", "your");
            }
            else if (body.Contains("{item:$owner}'s") && Owner.IsViewer)
            {
                body = body.Replace("{item:$owner}'s", "your");
            }

            return body.Replace("$type", $"{_activity.TemplateObjectType}");
        }

        private uint GetFeedType() {
            switch (subject_type) {
                case "sitepage":
                case "sitegroup":
                    return 1;
                case "user":
                    return 0;
                default:
                    return 1;
            }
        }

        private FeedAttachment[] GetAttachments(Activity activity, ulong forSource, string sourceType) {
            if (activity.RawActivity.Attachments == null) return null;

            var feedAttachments = new List<FeedAttachment>();
            //if it's a like/hare or comment, then the attachment is the subject feed

            if (activity.ObjectType == "activity_action")
                feedAttachments.Add(new FeedAttachmentActivity(activity.InnerActivity, activity.RawActivity.object_id,
                    forSource, sourceType,_isIOS));
            else
                foreach (var att in activity.RawActivity.Attachments)
                {
                    var attach = FeedAttachmentFactory.Create(att,activity.ActivityId);
                    if(attach != null) feedAttachments.Add(attach);
                }

            if (feedAttachments.Count > 0)
                return feedAttachments.ToArray();
            else
                return null;
        }


        private FeedWordStile[] GetWordStyling() {
            if (!string.IsNullOrEmpty(body))
                return WordStylings.GetAll().Where(s => body.Contains(s.title))
                    .Select(s=>new FeedWordStile(s)/* => JsonConvert.SerializeObject(s)*/).ToArray();
            return new FeedWordStile[0];
        }

        private object GetFeedDecorationSetting() {
            //if (_activity.IsPrimary) // || _activity.RawActivity.object_type != "action_activity")
            //    return new string[0];

            var ret = new {
                char_length = CoreSettings.Get("advancedactivity.feed.char.length", "50"),
                font_size = CoreSettings.Get("advancedactivity.feed.font.size", "20"),
                font_color = CoreSettings.Get("advancedactivity.feed.font.color", "#000"),
                font_style = CoreSettings.Get("advancedactivity.feed.font.style", "normal")
            };
            return ret; // JsonConvert.SerializeObject(ret);
        }
    }

    public class FeedWordStile
    {  
        public string background_color;
        public string color;
        public object @params;
        public string style;
        public string title;
        public int word_id;


        public FeedWordStile(WordStyling s)
        {
            background_color = s.background_color;
            color = s.color;
            @params = JsonConvert.DeserializeObject(s.@params);
            style = s.style;
            title = s.title;
            word_id = s.word_id;
        }
    }

    public class ActionTypeTransformer
    {
        private static Dictionary<string, string> _transformTable;
        private static object _lock = new object();

        public static string Transform(string actionType)
        {
            var ret = actionType;
            lock (_lock)
            {
                if (_transformTable == null)
                {
                    _transformTable = new Dictionary<string, string>();
                    _transformTable["{actors:$subject:$object}:\r\n{body:$body}"] = "{item:$subject} \u2192 {item:$object}:\r\n{body:$body}";
                    _transformTable["{item:$subject} is following {item:$owner}'s {item:$object:group}: {body:$body}"] = "{item:$subject} is following {item:$object}:\r\n{body:$body}";
                    _transformTable["{item:$subject} was at {item:$object} {var:$event_date}: {body:$body}"] = "{item:$subject} was at {item:$object}'s event.\r\n{body:$body}";
                }
            }

            if (_transformTable.ContainsKey(actionType))
            {
                ret = _transformTable[actionType];
            }

            //if (!ret.Contains("{body:$body}"))
            //{
            //    ret = ret + "\r\n{body:$body}";
            //}

            return ret; 
        }
    }

    internal class ItemUtil
    {
        internal static string GetItemLabel(string template)
        {
            var m = Regex.Match(template, @"{item:\$object:[a-z]+}");
            if (string.IsNullOrEmpty(m.Value)) return "";

            string res = m.Value.Replace("{item:$object:", "").Replace("}", "");
            return res;
        }
    }
}
 