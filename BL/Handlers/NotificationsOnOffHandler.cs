using System;
using System.Collections.Generic;
using System.Text;
using KoobecaFeedController.BL.Request;
using KoobecaFeedController.DAL.Models;

namespace KoobecaFeedController.BL.Handlers
{
    public class NotificationsOnOffHandler : ActivityHandler
    {
        public NotificationsOnOffHandler(uint userId, RequestParams reqParams) : base(userId, reqParams)
        {
        }

        public override void Execute()
        {
            //get notifications record for action_id
            var record = DAL.Adapters.NotifiedUsers.GetRecord(reqParams.action_id);

            if (record == null)
            {
                //if recode does not exist, add it with user and set user as notified in activity
                NotifiedUser newRecord = new NotifiedUser() { action_id = (int)reqParams.action_id, user_ids = $"[{userId}]" };
                activity.NotifiedUsers.Add(userId);
                DAL.Adapters.NotifiedUsers.Add(newRecord);
            }
            else
            {
                if (!record.ContainsUser(userId))
                {
                    //if record exists and not containing the user, add the user and set as notified in activity
                    record.AddUser(userId);
                    DAL.Adapters.NotifiedUsers.Update(record);
                    activity.NotifiedUsers.Add(userId);
                }
                else
                {
                    //if record exists and contains the user, remove the user and remove it from the activity
                    record.RemoveUser(userId);
                    if(string.IsNullOrEmpty(record.user_ids))
                    {
                        DAL.Adapters.NotifiedUsers.Remove((uint)record.action_id);
                    }
                    else
                    {
                        DAL.Adapters.NotifiedUsers.Update(record);
                    }
                    activity.NotifiedUsers.Remove(userId);
                }
            }
        }
    }
}
