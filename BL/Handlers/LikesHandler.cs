using KoobecaFeedController.BL.Request;
using KoobecaFeedController.DAL.Adapters;
using KoobecaFeedController.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KoobecaFeedController.BL.Handlers
{
    public class LikesHandler : AddRefHandler
    {

        public override void Execute()
        {
            Logger.Instance.Debug($"got reaction {reqParams.reaction}");
            base.Execute();
        }

        

        public LikesHandler(uint userId, RequestParams reqParams) :base(userId, reqParams, ActivityType.Like)
        {}

        protected override void CheckIfRefExists(out ActivityAction refActivity, out object activityRecord)
        {
            refActivity = ActivityActions.GetRefActivity("like_activity_action", activity.ActivityId, "activity_action", userId, false);
            activityRecord = Likes.GetByResourceAndPoster((uint)activity.ActivityId, "activity_action", userId, "user");

            if (refActivity != null && !IsActivityUpdate(refActivity))
            {
                throw new Exception($"Like to {activity.ActivityId} by user {userId} already exists in activity actions");
            }
            if (activityRecord != null && !IsRecordUpdate(activityRecord))
            {
                throw new Exception($"Like to {activity.ActivityId} by user {userId} already exists in Likes table");
            }
        }

        protected override void AddUpdateRefItemToSource(ActivityAction refActivity,string reaction)
        {
            userSource.LikedItems.React(refActivity.object_type, refActivity.object_id,reaction);
        }

        protected override void AddRefToTable()
        {
            activity.Reactions[reqParams.reaction]++;
            Likes.Add(new Like() { poster_id = userId, poster_type = "user", reaction = reqParams.reaction, resource_id = (uint)activity.ActivityId });
        }

        protected override void UpdateRefInTable(Like oldRecord)
        {
            activity.Reactions[oldRecord.reaction]--;
            activity.Reactions[reqParams.reaction]++;
            Likes.UpdateReaction(userId, activity.ActivityId, reqParams.reaction);
        }

        protected override void IncreaseCount()
        {
            activity.RawActivity.like_count++;
        }

        protected override bool IsActivityUpdate(ActivityAction refActivity)
        {
            Logger.Instance.Debug($"current body is '{refActivity.BodyStr}' and new reaction is '{reqParams.reaction}'");
            return refActivity.BodyStr != reqParams.reaction;
        }

        protected override bool IsRecordUpdate(object activityRecord)
        {
            Like likeRecord = activityRecord as Like;
            Logger.Instance.Debug($"current like record reaction is '{likeRecord.reaction}' and new reaction is '{reqParams.reaction}'");
            return likeRecord.reaction != reqParams.reaction;
        }

        protected override byte[] GetRefBody()
        {
            return Encoding.ASCII.GetBytes(reqParams.reaction);
        }

        protected override void SetReqParamsDefaults()
        {
            if (string.IsNullOrEmpty(reqParams.reaction)) reqParams.reaction = "like";//default reaction
        }

        protected override ActivityNotification CreateNotification()
        {
            var notification = base.CreateNotification();
            notification.type = "liked";
            return notification;
        }

        protected override bool IsAuthorized
        {
            get
            {
                return (this.authorization & (ushort)AuthorizationFlags.Like) > 0;
            }
        }
    }
}
