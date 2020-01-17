using KoobecaFeedController.DAL.Adapters;
using System;
using System.Collections.Generic;
using System.Text;

namespace KoobecaFeedController.BL
{
    public class ActivityCache
    {
        private Dictionary<ulong, ActivityCacheItem> _cache = new Dictionary<ulong, ActivityCacheItem>();
        private object _lock = new object();

        public static ActivityCache Instance { get; } = new ActivityCache();

        private ActivityCache() { }

        public Activity Get(ulong actionId)
        {
            lock (_lock)
            {
                if (_cache.TryGetValue(actionId, out ActivityCacheItem item))
                {
                    item.LastAccess = DateTime.Now;
                    return item.Activity;
                }
            }
            //else
            var newitem = new ActivityCacheItem();
            var action = ActivityActions.GetById((uint)actionId);
            if (action != null)
            {
                newitem.Activity = new Activity(action);
            }
            lock (_lock)
            {
                _cache[actionId] = newitem;
            }
            return newitem.Activity;
        }

        public void Set(Activity newActivity)
        {
            lock (_lock)
            {
                if (!_cache.ContainsKey(newActivity.ActivityId))
                {
                    _cache[newActivity.ActivityId] = new ActivityCacheItem() { Activity = newActivity };
                }
            }
        }

        public void Remove(ulong actionId)
        {
            lock (_lock)
            {
                if (_cache.ContainsKey(actionId))
                {
                    _cache.Remove(actionId);
                }
            }
        }

        internal void Nullify(ulong actionId)
        {
            lock (_lock)
            {
                _cache[actionId] = new ActivityCacheItem();
            }
        }
    }

    public class ActivityCacheItem
    {
        public Activity Activity { get; set; }
        public DateTime LastAccess { get; internal set; } = DateTime.Now;
    }
}
