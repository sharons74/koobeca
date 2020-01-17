using System;
using System.Collections.Generic;
using System.Text;
using KoobecaFeedController.BL.Request;
using KoobecaFeedController.DAL.Adapters;
using KoobecaFeedController.DAL.Models;

namespace KoobecaFeedController.BL.Handlers
{
    public class SaveActivityHandler : ActivityHandler
    {
        public SaveActivityHandler(uint userId, RequestParams reqParams) : base(userId, reqParams)
        {
        }

        public override void Execute()
        {
            var savedActivity = SavedActivities.Get((int)userId, (int)activity.ActivityId);
            if (savedActivity == null)
            {
                Logger.Instance.Debug($"Saving activity {activity.ActivityId} of type {activity.RawActivity.type} for user {userId}");
                Response = SavedActivities.Add(new SavedActivity() { user_id = (int)userId,action_type = activity.RawActivity.type,action_id= (int)activity.ActivityId}).ToString();
                if(Response == "1")
                {
                    userSource.SavedItems.Save(reqParams.action_id);
                }
            }
            else
            {
                //unsave
                Logger.Instance.Debug($"UnSaving activity {activity.ActivityId} of type {activity.RawActivity.type} for user {userId}");
                Response = SavedActivities.Remove((int)userId,(int)activity.ActivityId).ToString();
                if (Response == "1")
                {
                    userSource.SavedItems.Unsave(reqParams.action_id);
                }
            }
        }
    }
}
