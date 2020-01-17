using KoobecaFeedController.BL.Rendering;
using KoobecaFeedController.BL.Request;
using KoobecaFeedController.DAL.Adapters;
using KoobecaFeedController.DAL.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KoobecaFeedController.BL.Handlers
{
    public class CommentHandler : AddRefHandler
    {
        private Dictionary<string, object> additionalParams;

        private Comment comment;

        public CommentHandler(uint userId,RequestParams reqParams,Dictionary<string,object> additionalParams) : base(userId,reqParams,ActivityType.Comment)
        {
            this.additionalParams = additionalParams;
        }

        public override void Execute()
        {
            base.Execute();
            if(reqParams.GetComposer() != null)
            {
                Logger.Instance.Debug($"Found {reqParams.GetComposer().GetTaggedUsers().Count} tagged users");
            }
            var commentInfo = new CommentInfo(this.comment,userId);
            Response = JsonConvert.SerializeObject(commentInfo);
            Logger.Instance.Debug(Response);
        }


        protected override void CheckIfRefExists(out ActivityAction refActivity, out object activityRecord)
        {
            Logger.Instance.Debug($"comment id = {reqParams.comment_id}");
            refActivity = null;
            activityRecord = null;
            //Comment comment = Comments.GetByCommentId(reqParams.comment_id);
            //activityRecord = comment;
            //refActivity = null;

            //if (comment != null && !IsRecordUpdate(activityRecord))
            //{
            //    throw new Exception($"Comment to {activity.ActivityId} by user {userId} already exists in Comments table");
            //}
            //else if (comment != null)
            //{
            //    refActivity = ActivityActions.GetRefActivity("comment_activity_action", activity.ActivityId, "activity_action", userId, false, comment.creation_date);
            //}

            //if (refActivity != null && !IsActivityUpdate(refActivity))
            //{
            //    throw new Exception($"Comment to {activity.ActivityId} by user {userId} already exists in activity actions");
            //}
        }

        protected override bool IsActivityUpdate(ActivityAction refActivity)
        {
            Logger.Instance.Debug($"current body is '{refActivity.BodyStr}' and new reaction is '{reqParams.body}'");
            return !refActivity.body.SequenceEqual(GetRefBody());
        }


        protected override void AddRefToTable()
        {
            comment = new Comment()
            {
                body = GetRefBody(),
                creation_date = DateTime.Now,
                poster_id = userId,
                poster_type = "user",
                resource_id = (uint)activity.ActivityId ,
            };
            comment.attachment_type = additionalParams.ContainsKey("attachment_type") ? (string)additionalParams["attachment_type"] : null;
            comment.attachment_id = additionalParams.ContainsKey("attachment_id") ?(int)((long)additionalParams["attachment_id"]):0;
            if(additionalParams != null)
            {
                comment.@params = GetCommentParams(additionalParams);
            }
            Logger.Instance.Info($"size is {comment.body.Length} body and {comment.@params.Length} params");

            Comments.Add(comment);
        }

        private string GetCommentParams(Dictionary<string, object> additionalParams)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            var keys = new List<string> { "image", "image_normal" , "image_profile" , "image_icon" , "content_url" ,
                "author_image" , "author_image_normal" , "author_image_profile" , "author_image_icon" };
            foreach (var key in keys){
                try {
                    parameters[key] = (string)additionalParams[key];
                }
                catch(Exception e)
                {
                    Logger.Instance.Error($"{key} not found in additionalParams {e.ToString()}");
                }
            }

            return JsonConvert.SerializeObject(parameters);
        }

        protected override void IncreaseCount()
        {
            activity.RawActivity.comment_count++;
        }
        

        protected override ActivityNotification CreateNotification()
        {
            var notification = base.CreateNotification();
            notification.type = "commented";
            return notification;
        }

        protected override void SendNotifications()
        {
            base.SendNotifications();
            //also send notifications for tagges users
            var composer = reqParams.GetComposer();
            if(composer != null)
            {
                foreach(var taggedUser in composer.GetTaggedUsers())
                {
                    Logger.Instance.Debug($"sending notification to tagged user {taggedUser}");
                    var tagNotification = CreateNotification();
                    tagNotification.user_id = (uint)taggedUser;
                    tagNotification.type = "tagged";
                    tagNotification.subject_id = userId;
                    tagNotification.@params = "{\"object_type_name\":\"activity_comment\",\"label\":\"comment\"}";
                    ActivityNotifications.Add(tagNotification);
                }
            }
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
