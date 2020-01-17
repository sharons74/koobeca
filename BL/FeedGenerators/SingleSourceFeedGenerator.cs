using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KoobecaFeedController.BL.Rendering;
using KoobecaFeedController.BL.Request;

namespace KoobecaFeedController.BL.FeedGenerators
{
    public class SingleSourceFeedGenerator : FeedGenerator
    {
        protected string type;
        protected ulong sourceId;
        protected SourceData source;

        public SingleSourceFeedGenerator(RequestParams request) : base(request)
        {
            Logger.Instance.Debug($"Getting feed of {request.subject_type}:{request.subject_id}");
            source = SourcesMap.Instance.GetSource(request.subject_id, request.subject_type);
            timeSliceSource = source;
        }


        protected override SourceData[] GetFeedSources()
        {
            return new SourceData[] { source };
        }

        protected override IEnumerable<CombinedActivity> GetFeed()
        {
            //do single feed activity first
            feedFilter.StartDate = DateTime.MinValue; //the first take is the feed

            var feed = source.ActivityFeed.Feed;

            if (feed.Count == 0)
            {
                //no point to furthere iterations
                Logger.Instance.Debug($"feed is empty , so no point for further iterations");
                return new List<CombinedActivity>();
            }
            //get source activities filtered
            for (int i = feed.Count - 1 ; i > 0; i--)
            {
                var activity = ActivityCache.Instance.Get(feed[i]);
                if (feedFilter.Match(activity,out bool tooYoung))
                {
                    AddActivityToCombined(activity);

                    if (tooYoung || combinedActivities.Count >= feedSizeLimit)
                        break;
                }
            }

            Logger.Instance.Debug($"got {combinedActivities.Count} combined activities in first take");

            

            if (combinedActivities.Count < feedSizeLimit)
            {
                feedFilter.EndDate = source.ActivityFeed.MinimumDate;
                feedFilter.StartDate = feedFilter.EndDate.AddDays(-TimeSlice);
            }

            //now do the regular
            return base.GetFeed();
        }
    }
}
