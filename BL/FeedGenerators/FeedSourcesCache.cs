using System;
using System.Collections.Generic;
using System.Linq;

namespace KoobecaFeedController.BL.FeedGenerators
{
    public class FeedSourcesCache
    {
        private static Dictionary<string, FeedSourcesCacheItem> _Cache = new Dictionary<string, FeedSourcesCacheItem>();
        private static int _CacheMaxAge = 60; //minutes
        private static object _Lock = new object();
        public static List<SourceData> GetCache(string cacheKey)
        {
            lock (_Lock)
            {
                if (_Cache.ContainsKey(cacheKey))
                {
                    var item = _Cache[cacheKey];
                    var span = DateTime.Now - item.Date;
                    if (span.TotalMinutes < _CacheMaxAge)
                    {
                        return item.Data.ToList();
                    }
                    else
                    {
                        _Cache[cacheKey] = null;
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public static void SetCache(string cacheKey, List<SourceData> res)
        {
            var item = new FeedSourcesCacheItem()
            {
                Data = res.ToArray(),
                Date = DateTime.Now
            };
            lock (_Lock)
            {
                _Cache[cacheKey] = item;
            }
        }
    }

    public class FeedSourcesCacheItem
    {
        public DateTime Date { get; internal set; }
        public SourceData[] Data { get; set; }
    }
}