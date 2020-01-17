using KoobecaFeedController.BL.Rendering;
using KoobecaFeedController.BL.Request;
using KoobecaFeedController.DAL.Adapters;
using KoobecaFeedController.DAL.Models;
using Newtonsoft.Json;
using System.Linq;

namespace KoobecaFeedController.BL.Handlers
{
    internal class ActivityNotificationsHandler : ActivityHandler
    {
        private uint _userId;

        public ActivityNotificationsHandler(uint userId, RequestParams reqParams) : base(userId, reqParams)
        {
            _userId = userId;
        }

        public override void Execute()
        {
            var notifications = ActivityNotifications.GetByUserId(_userId).Select(n => new NotificationWrapper(n)).ToArray(); ;
            foreach(var notification in notifications)
            {
                if(notification.object_type == "activity_action")
                {
                    var activity = ActivityActions.GetById(notification.object_id);
                    Feed f = new Feed(new Activity(activity), notification.subject_id, notification.subject_type, false);
                    notification._object =  new ActivityActionWrapper(activity);
                    notification.feed_title = f.feed_title;
                    notification.action_type_body_params = f.action_type_body_params;
                    notification.url = f.Subject.Url;
                }
                else
                {

                }
            }


            var response = new
            {
                recentUpdateTotalItemCount = notifications.Length,
                recentUpdates = notifications
            };

            Response = JsonConvert.SerializeObject(response);
        }
    }

    public class NotificationWrapper : ActivityNotification
    {
        public object subject = new object();
        internal string feed_title;
        internal FeedActionTypeBodyParam[] action_type_body_params;
        internal string url;

        [JsonProperty(PropertyName = "object")]
        public object _object { get; set; }

        public NotificationWrapper(ActivityNotification notification)
        {
            this.date = notification.date;
            this.mitigated = notification.mitigated;
            this.notification_id = notification.notification_id;
            this.object_id = notification.object_id;
            this.object_type = notification.object_type;
            this.read = notification.read;
            this.show = notification.show;
            this.user_id = notification.user_id;
            this.type = notification.type;
            this.@params = notification.@params;
            this.subject_id = notification.subject_id;
            this.subject_type = notification.subject_type;
            
        }
    }

    public class ActivityActionWrapper : ActivityAction
    {
        public ActivityActionWrapper(ActivityAction a)
        {
            this.action_id = a.action_id;
            this.type = a.type;
            this.subject_type = a.subject_type;
            this.subject_id = a.subject_id;
            this.object_type = a.object_type;
            this.object_id = a.object_id;
            this.body = a.body;      
            this.@params = a.@params;
            this.date = a.date;
            this.modified_date = a.modified_date;
            this.attachment_count = a.attachment_count;
            this.comment_count = a.comment_count;
            this.like_count = a.like_count;
            this.privacy = a.privacy;
            this.commentable = a.commentable;
            this.shareable = a.shareable;
            this.user_agent = a.user_agent;
            this.publish_date = a.publish_date; 




        }

        //"url":"http:\/\/beta.koobeca.com\/profile\/3\/action_id\/110320",
        //    "image":"http:\/\/beta.koobeca.com\/\/application\/modules\/User\/externals\/images\/nophoto_user_thumb_profile.png",
        //    "image_icon":"http:\/\/beta.koobeca.com\/\/application\/modules\/User\/externals\/images\/nophoto_user_thumb_icon.png",
        //    "image_profile":"http:\/\/beta.koobeca.com\/\/application\/modules\/User\/externals\/images\/nophoto_user_thumb_profile.png",
        //    "image_normal":"http:\/\/beta.koobeca.com\/\/application\/modules\/User\/externals\/images\/nophoto_user_thumb_profile.png",
        //    "content_url":"http:\/\/beta.koobeca.com\/profile\/3\/action_id\/110320",
        //    "reactionsEnabled":1,
        //    "feed_reactions":[
    }
}