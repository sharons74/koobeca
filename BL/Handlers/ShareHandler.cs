using System;
using System.Collections.Generic;
using System.Text;
using KoobecaFeedController.BL.Request;
using KoobecaFeedController.DAL.Adapters;
using KoobecaFeedController.DAL.Models;

namespace KoobecaFeedController.BL.Handlers
{
    public class ShareHandler : ActivityHandler
    {
        public ShareHandler(uint userId, RequestParams reqParams) : base(userId, reqParams)
        {
        }

        public override void Execute()
        {
            Logger.Instance.Debug($"Sharing activity {reqParams.action_id} by user:{userId}");
            activity.Source.ActivityFeed.AdvanceActivity(activity);
            activity.RawActivity.modified_date = DateTime.Now;
            ActivityActions.Update(activity.RawActivity);
            if (activity.IsShare)
            {
                //we want to share the primary
                activity = activity.InnerActivity;
            }
            var share = CreateRefActivity(ActivityType.Share);
            userSource.ActivityFeed.AddActivity(share);
            SendNotifications();
            LikeSourceIfPossible(activity.EffectiveSubjectId, activity.EffectiveSubjetType); 
        }

        protected override ActivityNotification CreateNotification()
        {
            var notification = base.CreateNotification();
            notification.type = "shared";
            notification.@params = notification.@params.Replace("TYPE", "post");
            return notification;
        }

        protected override bool IsAuthorized
        {
            get
            {
                Logger.Instance.Debug($"User authorization is {this.authorization}");
                return (this.authorization & (ushort)AuthorizationFlags.Share) > 0;
            }
        }
    }
}
