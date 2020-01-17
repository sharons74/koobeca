using KoobecaFeedController.BL.Request;
using KoobecaFeedController.DAL.Adapters;
using KoobecaFeedController.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KoobecaFeedController.BL.Handlers
{
    public class UnlikeHandler : RemoveRefHandler
    {
        public UnlikeHandler(uint userId,RequestParams reqParams) : base(userId,reqParams, ActivityType.Like) { }

        protected override void DecreaseCount()
        {
            activity.RawActivity.like_count--;
        }

        protected override void RemoveRecordFromTable(object refRecord)
        {
            Like like = refRecord as Like;
            activity.Reactions[like.reaction]--;
            Likes.Remove(like.poster_id, like.resource_id);
        }

        protected override void RemoveRefItemsFromSource()
        {
            userSource.LikedItems.UnLike("activity_action",(uint)activity.ActivityId);
        }

        protected override void CheckIfRefNotExists( out ActivityAction refActivityAction, out object actionRecord)
        {
            refActivityAction = ActivityActions.GetRefActivity("like_activity_action", activity.ActivityId, "activity_action", userId, true);
            actionRecord = Likes.GetByResourceAndPoster((uint)activity.ActivityId, "activity_action", userId, "user");


            if ((actionRecord == null && refActivityAction != null) || (actionRecord != null && refActivityAction == null))
            {
               Logger.Instance.Warn($"Inconsistancy in likes and activity tables, to {activity.ActivityId} by user {userId}");
            }

            if (refActivityAction == null)
            {
                Logger.Instance.Warn($"like to {activity.ActivityId} by user {userId} not exists in activity actions");
            }
            if (actionRecord == null)
            {
                Logger.Instance.Warn($"like to {activity.ActivityId} by user {userId} not exists in likes table");
            }
        }

    }
}
