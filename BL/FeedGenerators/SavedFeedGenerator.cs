using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KoobecaFeedController.BL.Request;
using KoobecaFeedController.DAL.Adapters;

namespace KoobecaFeedController.BL.FeedGenerators
{
    public class SavedFeedGenerator : FeedGenerator
    {
        public SavedFeedGenerator(RequestParams request) : base(request)
        {
        }

        protected override IEnumerable<CombinedActivity> GetFeed()
        {
            return viewer.SavedItems.Select(a => new CombinedActivity() { PrimeActivity = ActivityCache.Instance.Get(a) });
        }

        protected override SourceData[] GetFeedSources()
        {
            return null;
        }
    }
}
