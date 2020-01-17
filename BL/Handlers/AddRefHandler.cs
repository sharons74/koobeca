using System;
using System.Collections.Generic;
using System.Text;
using KoobecaFeedController.BL.Request;
using KoobecaFeedController.DAL.Adapters;
using KoobecaFeedController.DAL.Models;

namespace KoobecaFeedController.BL.Handlers
{
    public class AddRefHandler : ActivityHandler
    {
        protected ActivityType type;

        

        public AddRefHandler(uint userId,RequestParams reqParams, ActivityType type) : base(userId, reqParams)
        {
            this.type = type;
        }

        public override void Execute()
        {
            SetReqParamsDefaults();

            CheckIfRefExists(out ActivityAction refActivityAction,out object activityRecord);

            
            Activity refActivity = null;
            if (refActivityAction == null)
            {
                //adding new referenc activity
                IncreaseCount();
                activity.Source.ActivityFeed.AdvanceActivity(activity); // advance the activity that is reference in it's ownr's feed
                activity.RawActivity.modified_date = DateTime.Now;
                ActivityActions.Update(activity.RawActivity); //update DB
                refActivity = CreateRefActivity(type);
                userSource.ActivityFeed.AddActivity(refActivity);
                AddUpdateRefItemToSource(refActivity.RawActivity,reqParams.reaction);
                AddRefToTable();
                SendNotifications();
                LikeSourceIfPossible(activity.EffectiveSubjectId,activity.EffectiveSubjetType);
            }
            else
            {
                //update content of existing activity reference
                Logger.Instance.Debug($"Update content of existing activity {refActivityAction.action_id}");
                activity.Source.ActivityFeed.AdvanceActivity(activity); // advance the activity that is reference in it's ownr's feed
                activity.RawActivity.modified_date = DateTime.Now;
                ActivityActions.Update(activity.RawActivity); //update DB
                refActivity = ActivityCache.Instance.Get(refActivityAction.action_id);
                refActivity.RawActivity.body = GetRefBody();
                AddUpdateRefItemToSource(refActivity.RawActivity, reqParams.reaction);
                UpdateActionInDB(refActivity.RawActivity);
                UpdateRefInTable((Like)activityRecord);
            }
        }

        protected virtual void SetReqParamsDefaults() {}

        protected virtual void UpdateRefInTable(Like oldLike) {}

        protected virtual void AddUpdateRefItemToSource(ActivityAction rawActivity,string reaction) {}

        protected virtual void AddRefToTable() {}

        protected virtual void IncreaseCount() {}

        protected override ActivityNotification CreateNotification()
        {
            var notification = base.CreateNotification();
            notification.@params = notification.@params.Replace("TYPE", "post");
            return notification;
        }



        protected virtual void CheckIfRefExists(out ActivityAction refActivity,out object activityRecord)
        {
            activityRecord = null;
            refActivity = null;
        }

        protected virtual bool IsActivityUpdate(ActivityAction refActivity)
        {
            return false;
        }

        protected virtual bool IsRecordUpdate(object activityRecord)
        {
            return false;
        }

    }
}
