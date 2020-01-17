using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KoobecaFeedController.BL.Rendering;
using KoobecaFeedController.BL.Request;
using KoobecaFeedController.DAL.Adapters;

namespace KoobecaFeedController.BL.Handlers
{
    public class UpdateHandler : ActivityHandler
    {
        public UpdateHandler(uint userId, RequestParams reqParams) : base(userId, reqParams)
        {
        }

        public override void Execute()
        {
            //Logger.Instance.Debug($"action id is {activity.ActivityId}");
            ActivityCache.Instance.Remove(activity.ActivityId);
            activity.Source.ActivityFeed.AdvanceActivity(activity);
            SetLinkParent();
            
            //we create the feedwrapper for the activity. this will put all needed resources of DB in cache
            FeedWrapper.Create(activity, activity.SubjectId, activity.SubjectType,false);
        }

        private void SetLinkParent()
        {
            var attachments = activity.RawActivity.Attachments;

            if (attachments == null) return;

            foreach(var linkId in attachments.Where(a => a.type == "core_link").Select(a => a.id))
            {
                Logger.Instance.Debug($"setting parent of link {linkId} to action {activity.ActivityId}");
                var link = CoreLinks.GetById(linkId);
                link.parent_id = (uint)activity.ActivityId;
                link.parent_type = "activity_action";

                CoreLinks.UpdateParent(linkId, activity.ActivityId, "activity_action");
            }
        }
    }
}
