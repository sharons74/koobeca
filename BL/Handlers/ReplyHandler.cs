using System;
using System.Collections.Generic;
using System.Text;
using KoobecaFeedController.BL.Request;
using KoobecaFeedController.DAL.Adapters;
using KoobecaFeedController.DAL.Models;
using Newtonsoft.Json;

namespace KoobecaFeedController.BL.Handlers
{
    public class ReplyHandler : ActivityHandler
    {
        private Dictionary<string, object> additionalParams;
        private ulong commentId;
        private Comment parentComment;

        public ReplyHandler(uint userId, RequestParams reqParams, Dictionary<string, object> additionalParams) : base(userId, reqParams)
        {
            this.additionalParams = additionalParams;
            commentId = (ulong)(long)additionalParams["commentId"];
        }

        public override void Execute()
        {
            Logger.Instance.Debug($"updating comment {commentId}");
            var res = Comments.UpdateParams((uint)commentId, JsonConvert.SerializeObject(additionalParams));
            if(res > 0)
            {
                Logger.Instance.Debug($"Success !");
            }

            if(reqParams.comment_id != 0)
            {
                Logger.Instance.Debug($"sending reply notification");
                SendNotifications();
            }
        }

        protected override void SendNotifications()
        {
            parentComment = Comments.GetById((uint)reqParams.comment_id);
            var notification = CreateNotification();
            if (notification.subject_id == notification.user_id)
            {
                Logger.Instance.Debug($"user:{userId} will not be notified on his own actions");
                return;
            }
            //Logger.Instance.Debug($"sending notification:{JsonConvert.SerializeObject(notification)}");
            ActivityNotifications.Add(notification);
        }

        protected override ActivityNotification CreateNotification()
        {
            var notification = base.CreateNotification();
            notification.user_id = parentComment.poster_id;
            notification.object_type = "activity_comment";
            notification.object_id = parentComment.comment_id;
            notification.type = "replied";
            notification.@params = notification.@params.Replace("TYPE", "comment");
            return notification;
        }
    }
}
