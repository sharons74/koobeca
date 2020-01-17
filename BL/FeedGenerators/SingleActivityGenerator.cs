using System;
using System.Collections.Generic;
using System.Text;
using KoobecaFeedController.BL.Request;

namespace KoobecaFeedController.BL.FeedGenerators
{
    public class SingleActivityGenerator : FeedGenerator
    {
        ulong actionId;

        public SingleActivityGenerator(RequestParams request) : base(request)
        {
            Logger.Instance.Debug($"Getting feed of activity {request.action_id}");
            actionId = request.action_id;
        }

        protected override IEnumerable<CombinedActivity> GetFeed()
        {
            //get the activity
            var activity =  ActivityCache.Instance.Get(actionId);
            return new List<CombinedActivity>() { new CombinedActivity() { PrimeActivity = activity } };
        }
    }
}
