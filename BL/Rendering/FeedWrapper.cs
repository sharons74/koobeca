using System;
using System.Collections.Generic;
using System.Linq;
using KoobecaFeedController.DAL.Adapters;
using Newtonsoft.Json;

namespace KoobecaFeedController.BL.Rendering {
    public class FeedWrapper {
        public Feed feed;
        public uint? canComment;
        public uint? is_like;
        public uint? can_comment;
        public uint? can_share;
        public string feed_link;
        public bool isNotificationTurnedOn;
        public uint isSaveFeedOption;
        public Menus.Item[] feed_menus;
        public FooterMenus feed_footer_menus;
        public uint reactionsEnabled = 1;
        public Dictionary<uint, FeedReaction> feed_reactions;
        public FeedReaction my_feed_reaction;
        public object hashtags;

       // public FeedWrapper() { }


        private bool isIOS = false;

        public FeedWrapper(Activity activity, ulong forSource, string sourceType,bool isIOS) {
            var source = SourcesMap.Instance.GetSource(forSource, sourceType);
            this.isIOS = isIOS;

            feed = new Feed(activity, forSource, sourceType,isIOS);
           
            var tagList = feed.TagList;
            if (activity.IsLikeCommentOrReact)
            {
                //copy from inner feed
                FeedAttachmentActivity innerFeed = feed.attachment[0] as FeedAttachmentActivity;
                var innerWrapper = innerFeed.shared_post_data[0];
                canComment = can_comment = innerWrapper.canComment;
                can_share = innerWrapper.can_share;
                is_like = innerWrapper.is_like;
                isNotificationTurnedOn = innerWrapper.isNotificationTurnedOn;
                isSaveFeedOption = innerWrapper.isSaveFeedOption;
                feed_menus = innerWrapper.feed_menus;
                feed_footer_menus = innerWrapper.feed_footer_menus;
               // feed_link = innerWrapper.feed_link;
                //feed_link = $"https://beta.koobeca2.com/profile/{activity.EffectiveSubjectId}/action_id/{activity.ActivityId}/show_comments/1";
                feed_link = $"https://beta.koobeca.com/view.php?action_id={activity.ActivityId}";
                feed_reactions = innerWrapper.feed_reactions;
                my_feed_reaction = innerWrapper.my_feed_reaction;
            }
            else //primary or share
            {
                canComment = can_comment = feed.commentable;
                can_share = feed.shareable;
                is_like = (uint)(activity.Reaction(forSource) != null ? 1 : 0);
                isNotificationTurnedOn = activity.NotifiedUsers.IsNotified(forSource);
                isSaveFeedOption = (uint)(source.SavedItems.IsSaved(activity.ActivityId)?0:1);
                feed_menus = new Menus(activity, source, sourceType).Items;
                feed_footer_menus = new FooterMenus(activity, feed);
                //feed_link = $"https://beta.koobeca.com/profile/{activity.EffectiveSubjectId}/action_id/{activity.ActivityId}/show_comments/1";
                feed_link = $"https://beta.koobeca.com/view.php?action_id={activity.ActivityId}";
                hashtags = tagList.Count > 0 ? tagList : null;

                feed_reactions = GetFeedReactions(activity);
                my_feed_reaction = GetMyReaction(activity, forSource);
            }

            if (isIOS)
            {
                ReviseFeed();
            }
           // feed.SetActionId();
        }

        private void ReviseFeed()
        {
            if(feed.attachment != null && feed.attachment.Length > 0)
            {
                var innerFeedAttachment = feed.attachment[0] as FeedAttachmentActivity;
                if(innerFeedAttachment != null && innerFeedAttachment.shared_post_data[0].feed.attachment != null && innerFeedAttachment.shared_post_data[0].feed.attachment.Length > 0)
                {
                    var innerFeed = innerFeedAttachment.shared_post_data[0].feed;
                    feed.attachment = innerFeed.attachment;
                    feed.attachment_content_type = innerFeed.attachment_content_type;
                    feed.photo_attachment_count = innerFeed.photo_attachment_count;
                    feed.feed_icon = innerFeed.feed_icon;
                    //feed.subject_type = innerFeed.subject_type;
                    //feed.subject_id = innerFeed.subject_id;
                }
            }
            //var innerFeed = ((FeedAttachmentActivity)feed.attachment[0]).shared_post_data[0].feed;
        }

        private Feed GetFeed(Activity activity, ulong forSource, string sourceType)
        {
            var feed = new Feed(activity, forSource, sourceType,isIOS);


            if (isIOS && !activity.IsPrimary)
            {
                //we show the inner activity but keep the title
                var innerFeed = ((FeedAttachmentActivity)feed.attachment[0]).shared_post_data[0].feed;
                //copy things from upper feed
                innerFeed.body = feed.body + "\r\n----------------\r\n" + innerFeed.feed_title ;
                if (!innerFeed.action_type_body.Contains("{body:$body}"))
                {
                    innerFeed.body += ":\r\n" + innerFeed.body;
                }
                innerFeed.feed_title = feed.feed_title;// + "\n\r" + innerFeed.feed_title;
                innerFeed.action_type_body = feed.action_type_body;//.Replace("item:","topitem:").Replace("body:", "topbody:") + "\r\n" + innerFeed.action_type_body;
                //var paramList = innerFeed.action_type_body_params.ToList();// = feed.action_type_body_params;
                //foreach(var prm in feed.action_type_body_params)
                //{
                //    prm.search = prm.search.Replace("item:", "topitem:");
                //    paramList.Add(prm);
                //    if (prm.search.Contains("body:"))
                //    {
                //        prm.search = prm.search.Replace("body:", "topbody:");
                //        prm.label = feed.body;
                //    }
                //}
                innerFeed.action_type_body_params = feed.action_type_body_params;// paramList.ToArray();
                var bodyParam = innerFeed.action_type_body_params.Where(p => p.search == "{body:$body}").FirstOrDefault();
                if(bodyParam != null) bodyParam.label = innerFeed.body;
                return innerFeed;

            }
            //else
            return feed;
        }

        private FeedReaction GetMyReaction(Activity activity,ulong userId)
        {
            var type = activity.Reaction(userId);
            if (!string.IsNullOrEmpty(type))
            {
                var reaction = Reactions.GetByType(type);
                return new FeedReaction(reaction, 0);
            }
            //else
            return null;
        }

        private Dictionary<uint, FeedReaction> GetFeedReactions(Activity activity)
        {
            Dictionary<uint, FeedReaction> reactions = new Dictionary<uint, FeedReaction>();

            foreach (var reactionType in activity.Reactions.Where(r => r.Value > 0).Select(r => r.Key))
            {
                var reaction = Reactions.GetByType(reactionType);
                reactions[reaction.reactionicon_id] = new FeedReaction(reaction, (uint)activity.Reactions[reactionType]);
            }

            return reactions;
        }

        public FeedWrapper(CombinedActivity combinedActivity, ulong forSource, string sourceType,bool isIOS) : this(
            combinedActivity.RefActivities[combinedActivity.FirstReactor], forSource, sourceType,isIOS) { }

        [JsonIgnore]
        public List<string> TagList => feed.TagList;

        public static FeedWrapper Create(CombinedActivity combinedActivity, ulong forSource, string sourceType,bool isIOS)
        {
            Activity activity = null;
            //prefer shares, then comments, then reactions, then primary
            if (combinedActivity.Shares.Count > 0) activity = combinedActivity.Shares[0];
            else if (combinedActivity.Comments.Count > 0) activity = combinedActivity.Comments[0];
            else if (combinedActivity.RefActivities.Count > 0) activity = combinedActivity.RefActivities.First().Value;
            else activity = combinedActivity.PrimeActivity;
            //var activity = combinedActivity.RefActivities.Count > 0 ? combinedActivity.RefActivities.First().Value : combinedActivity.PrimeActivity;

            if (activity == null)
            {
                Logger.Instance.Warn($"no activities in given combined activity");
                return null;
            }
            return Create(activity,forSource,sourceType,isIOS);
        }

        public static FeedWrapper Create(Activity activity, ulong forSource, string sourceType,bool isIOS) {
            try {
                return new FeedWrapper(activity, forSource, sourceType,isIOS);
            }
            catch (Exception e) {
                Logger.Instance.Error($"Failed to create activity wrapper for {activity?.ActivityId}:{e.Message}.removing the activity");
                Logger.Instance.Debug(e.ToString());
                RemoveActivity(activity);
                return null;
            }
        }

        private static void RemoveActivity(Activity activity)
        {
            
            try
            {
                activity.Source.ActivityFeed.RemoveActivity((uint)activity.ActivityId, true);
                //ActivityActions.Delete((uint)activity.ActivityId, true);
                //ActivityCache.Instance.Nullify(activity.ActivityId);
            }
            catch (Exception e)
            {
                Logger.Instance.Error($"Failed to remove: {e.ToString()}");
            }
        }

        public class Menus {
            private readonly Activity _activity;
            private readonly List<string> _list = new List<string>
                {"feed_link","on_off_notification", "update_save_feed" };

            public Menus(Activity activity,SourceData source, string sourceType) {
                _activity = activity;
                 var authorization = activity.GetAuthorization(source.SourceId,source.SourceType);

                //if (activity.HiddenFor(source)) { }
                // _list.Add("un_hide"); //TODO the response of unhide make error on client
                if (activity.EffectiveSubjetType == "sitepage_page")
                {
                    _list.Add("hide");
                }
                
                if (activity.IsPrimaryOrShare) {
                    if((authorization & (ushort)AuthorizationFlags.Edit) > 0) _list.Add("edit_feed");
                    if ((authorization & (ushort)AuthorizationFlags.Delete) > 0) _list.Add("delete");
                    if ((authorization & (ushort)AuthorizationFlags.DisableComments) > 0) {
                        if (activity.RawActivity.commentable)
                            _list.Add("disable_comment");
                        else
                            _list.Add("un_disable_comment");
                    }
                    if ((authorization & (ushort)AuthorizationFlags.DisableShares) > 0)
                    {
                        if(activity.RawActivity.shareable)
                            _list.Add("lock_this_feed");
                        else
                            _list.Add("un_lock_this_feed");
                    }
                }
            }


            public Item[] Items {
                get {
                    return _list.Select(i => new Item((uint) _activity.ActivityId, (uint) _activity.SubjectId, i))
                        .ToArray();
                }
            }

            public class Item {
                public string label;
                public string name;
                public string url;
                public string feed_link;
                public UrlParams urlParams;

                public Item() { }

                public Item(uint actionId, uint profileId, string name) {
                    this.name = name.Replace("un_","");
                    label = Labels[name];
                    url = Urls[name];
                    urlParams = new UrlParams {action_id = actionId};
                    if (name == "hide") { urlParams.type = "activity_action"; }//; urlParams.id = actionId; }
                    //else if (name == "report_feed") { urlParams.type = "activity_action"; urlParams.hide_report = 1; }
                    else if (name == "")
                        feed_link =
                            $"https://beta.koobeca.com/profile/{profileId}/action_id/{actionId}"; ///show_comments/1"
                }

                public class UrlParams {
                    public uint action_id;
                    //public uint? id;
                    public string type;
                    public int? hide_report;
                }

                private static readonly Dictionary<string, string> Labels = new Dictionary<string, string> {
                    {"feed_link", "Copy Link"},
                    {"on_off_notification", "\ud83d\udd07 Turn Off Notification"},
                    {"update_save_feed", "\ud83d\udcce Save Feed"},
                    {"hide", "I don't like this \uD83D\uDC4E"},
                    {"un_hide", "Unhide"},
                    {"delete", "Delete Feed" },
                    //{"promote", "I like this \uD83D\uDC4D" },
                    { "edit_feed", "Edit Feed \ud83d\udcdd"},
                    {"disable_comment", "Disable Comments"},
                    {"un_disable_comment" , "Enable Comments"},
                    {"lock_this_feed" , "Disable Sharing"},
                    {"un_lock_this_feed" , "  Enable Sharing"}
                };

                private static readonly Dictionary<string, string> Urls = new Dictionary<string, string> {
                    {"feed_link", null},
                    {"on_off_notification", "advancedactivity/turn-on-off-notification"},
                    {"update_save_feed", "advancedactivity/update-save-feed"},
                    {"hide", "advancedactivity/feeds/hide-item"},
                    {"un_hide", "advancedactivity/feeds/un-hide-item"},
                    {"delete", "advancedactivity/delete"},
                    //{"promote", "advancedactivity/delete"},
                    {"edit_feed", "advancedactivity/edit-feed"},
                    {"disable_comment" , "advancedactivity/update-commentable"},
                    {"lock_this_feed" , "advancedactivity/update-shareable"},
                    {"un_disable_comment" , "advancedactivity/update-commentable"},
                    {"un_lock_this_feed" , "advancedactivity/update-shareable"}
                };
            }
        }

        public class FooterMenus {
            public Item like;
            public Item share;
            public Item comment;

            public FooterMenus(Activity activity,Feed feed) {
                

                var shareActionId = activity.ObjectType == "activity_action" ? activity.ObjectId : activity.ActivityId;
                var likeActionId = activity.ActivityId;
                var commentActionId = activity.ActivityId;

                if (activity.IsLikeOrReact) likeActionId = commentActionId = activity.ObjectId;

                if (feed.likeable == 1) like = new Item("like", likeActionId); //,activity.ObjectType);
                if (feed.shareable == 1 && activity.Privacy == Privacy.Everyone) share = new Item("share", shareActionId, activity.ObjectType);
                if (feed.commentable == 1) comment = new Item("comment", commentActionId, activity.ObjectType);
            }

            

            public class Item {
                public string name;
                public string label;
                public string url;
                public UrlParams urlParams;

                public Item(string name, ulong actionId, string type = null) {
                    this.name = name;
                    label = Labels[name];
                    url = Urls[name];
                    urlParams = new UrlParams { id = actionId };//, type = type};
                }

                public class UrlParams {
                    public string type = "activity_action";
                    public ulong id;
                }

                private static readonly Dictionary<string, string> Labels = new Dictionary<string, string> {
                    {"like", "Like"},
                    {"share", "Share"},
                    {"comment", "Comment"}
                };

                private static readonly Dictionary<string, string> Urls = new Dictionary<string, string> {
                    {"like", "advancedactivity/like"},
                    {"share", "activity/share"},
                    {"comment", "advancedactivity/comment"}
                };
            }
        }
    }
}