using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KoobecaFeedController.BL.Request;

namespace KoobecaFeedController.BL.FeedGenerators
{
    public class HiddenFeedGenerator : FeedGenerator
    {
        public HiddenFeedGenerator(RequestParams request) : base(request)
        {
        }

        protected override IEnumerable<CombinedActivity> GetFeed()
        {
            return viewer.HidenItems.Select(a => new CombinedActivity() { PrimeActivity = ActivityCache.Instance.Get(a) });
        }

        protected override SourceData[] GetFeedSources()
        {
            return null;
        }
    }
}
