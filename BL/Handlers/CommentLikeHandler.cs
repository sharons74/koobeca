using System;
using System.Collections.Generic;
using System.Text;
using KoobecaFeedController.BL.Request;
using KoobecaFeedController.DAL.Adapters;
using KoobecaFeedController.DAL.Models;

namespace KoobecaFeedController.BL.Handlers
{
    public class CommentLikeHandler : ActivityHandler
    {
        private Comment comment;

        public CommentLikeHandler(uint userId, RequestParams reqParams) : base(userId, reqParams)
        {
        }

        public override void Execute()
        {

            Logger.Instance.Debug($"handling like of comment {reqParams.comment_id}");
            //add like count in comment
            this.comment = Comments.GetByCommentId(reqParams.comment_id);
            comment.like_count++;
            Comments.UpdateLikeCount(comment.comment_id, comment.like_count);
            CoreLike like = new CoreLike()
            {
                creation_date = DateTime.Now,
                poster_id = userId,
                poster_type = "user",
                reaction = "like",
                resource_id = comment.comment_id,
                resource_type = "activity_comment"
            };
            var likeId = CoreLikes.Add(like);
            Logger.Instance.Debug($"like id = {likeId}");
            SendNotifications();
        }

        protected override void SendNotifications()
        {
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
            notification.user_id  = comment.poster_id;
            notification.object_type = "activity_comment";
            notification.object_id = comment.comment_id;
            notification.type = "liked";
            notification.@params = notification.@params.Replace("TYPE", "comment");
            return notification;
        }

        protected override bool IsAuthorized
        {
            get
            {
                return (this.authorization & (ushort)AuthorizationFlags.Comment) > 0;
            }
        }
    }
}
