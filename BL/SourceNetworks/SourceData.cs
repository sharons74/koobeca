using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KoobecaFeedController.DAL.Adapters;

namespace KoobecaFeedController.BL {
    public class SourceData {
        private readonly object _lock = new object();
        private ulong _categories;

        private ActivityFeed _feed;

        private Dictionary<byte, List<CategoryInfo>> _subCategories =
            new Dictionary<byte, List<CategoryInfo>>();

        private LikedItems _LikedItems;
        private HiddenItems _HiddenItems;
        private SavedItems _SavedItems;

        public uint FetchDate { get; set; }
        public ulong LastMinimalIdInFeed { get; set; } //the last minimal id we vied in our last feed

        public ulong SourceId { get; set; }
        public string SourceType { get; set; }

        public Dictionary<string, double> FeedTimeSlices = new Dictionary<string, double>();

        public ulong Languages { get; set; }

        public uint LastActivity { get; set; } // last time the user done any activity

        public IEnumerable<Activity> GetFilteredFeed(FeedFilter filter)
        {
            try
            {  
                bool canFetch = true;
                while (canFetch && ActivityFeed.MinimumDate > filter.StartDate)
                {
                    //fetch more
                    canFetch = ActivityFeed.AppendFeed(ActivityFeed.MinimumDate, 50);
                }

                var feed = ActivityFeed.Feed.ToArray();
                List<Activity> res = new List<Activity>();
                for (int i = feed.Length - 1; i >= 0; i--)
                {
                    var activity = ActivityCache.Instance.Get(feed[i]);
                    if (filter.Match(activity, out bool tooYoung))
                    {
                        res.Add(activity);
                    }
                    if (tooYoung)
                    {
                        break; //no point to continue
                    }
                }
                return res;
            }
            catch(Exception e) {
                Logger.Instance.Error($"failed to get filtered feed for {SourceType}:{SourceId} {e.Message}");
                Logger.Instance.Debug(e.ToString());
                return new List<Activity>(); //empty list
            }
        }

        public SourceType Type { get; set; }

        public LikedItems LikedItems {
            get {
                if (_LikedItems == null) _LikedItems = new LikedItems(SourceId, SourceType);
                return _LikedItems;
            }
        }

        public HiddenItems HidenItems
        {
            get
            {
                lock (_lock)
                {
                    if (_HiddenItems == null) _HiddenItems = new HiddenItems(SourceId, SourceType);
                }
                return _HiddenItems;
            }
        }

        public SavedItems SavedItems
        {
            get
            {
                lock (_lock)
                {
                    if (_SavedItems == null) _SavedItems = new SavedItems(SourceId, SourceType);
                }
                return _SavedItems;
            }
        }

        public ActivityFeed ActivityFeed {
            get {
                lock (_lock) {
                    if (_feed == null) _feed = new ActivityFeed(SourceId, SourceType);
                }

                return _feed;
            }
        }

        public bool IsActive =>
            LocalTime.CurrentIntSecond - LastActivity < GlobalSettings.ActiveUserMaximalInactivityTime;


        public void ClearAllCategories() {
            _categories = 0;
            _subCategories = new Dictionary<byte, List<CategoryInfo>>();
        }

        public void SetCategories(List<CategoryInfo> categories) {
            foreach (var category in categories) {
                _categories |= (ulong) 1 << category.Category;
                if (!_subCategories.ContainsKey(category.Category))
                    _subCategories[category.Category] = new List<CategoryInfo>();
                _subCategories[category.Category].Add(category);
            }
        }

        public List<CategoryInfo> GetCategories() {
            var retList = new List<CategoryInfo>();
            foreach (var list in _subCategories.Values) retList.AddRange(list);

            return retList;
        }

        public List<CategoryInfo> GetUpdatedCategories() {
            var result = new List<CategoryInfo>();

            foreach (var subCategories in _subCategories.Values)
            foreach (var ci in subCategories)
                if (ci.CheckAffinityUpdate(FetchDate))
                    result.Add(ci);

            return result;
        }
    }

    public class LikedItems {
        private Dictionary<string, string> _items;
        private readonly ulong _sourceId;
        private readonly string _sourceType;

        public LikedItems(ulong sourceId, string sourceType) {
            _sourceId = sourceId;
            _sourceType = sourceType;
        }

        public string Reaction(string objectType, uint objectId) {
            var key = objectType + objectId;
            if (!Items.ContainsKey(key)) {
                var reationRecord = Likes.GetByResourceAndPoster(objectId, objectType, (uint)_sourceId, _sourceType);
                Items[key] = reationRecord?.reaction??null;
                //Logger.Instance.Debug(
                //    $"getting like information for {objectType}:{objectType} by {_sourceId}:{_sourceType} resulted with {Items[key]}");
            }

            return Items[key];
        }

        public void React(string objectType, uint objectId,string reaction) {
            var key = objectType + objectId;
            Items[key] = reaction;
        }

        public void UnLike(string objectType, uint objectId) {
            var key = objectType + objectId;
            Items[key] = null;
        }

        private Dictionary<string, string> Items {
            get {
                if (_items == null) _items = new Dictionary<string, string>();

                return _items;
            }
        }
    }

    public class RelatedActivityList : IEnumerable<ulong>
    {
        private Dictionary<ulong, bool> _items;
        protected readonly ulong _sourceId;
        protected readonly string _sourceType;
        private object _lock = new object();
        public RelatedActivityList(ulong sourceId, string sourceType)
        {
            _sourceId = sourceId;
            _sourceType = sourceType;
        }

        protected virtual ulong[] GetFromDB()
        {
            throw new NotImplementedException();
        }

        protected virtual string Label
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        private Dictionary<ulong, bool> Items
        {
            get
            {
                lock (_lock)
                {
                    if (_items == null)
                    {
                        _items = new Dictionary<ulong, bool>();
                        if (_sourceType != "user") return _items;

                        foreach (var activity in GetFromDB())
                        {
                            _items[activity] = true;
                        }
                        if (_items.Count > 0) Logger.Instance.Debug($"user {_sourceId} has {_items.Count} {Label} items");
                    }
                }

                return _items;
            }
        }

        protected void Add(ulong actionId)
        {
            lock (_lock)
            {
                Logger.Instance.Debug($"adding {actionId} to the {Label} items of {_sourceId}");
                Items[actionId] = true;
            }
        }

        protected void Remove(ulong actionId)
        {
            lock (_lock)
            {
                if (Items.ContainsKey(actionId))
                {
                    Logger.Instance.Debug($"removing {actionId} from the {Label} items of {_sourceId}");
                    Items.Remove(actionId);
                }
            }
        }

        protected bool Exists(ulong actionId)
        {
            lock (_lock)
            {
                return Items.ContainsKey(actionId);
            }
        }

        public IEnumerator<ulong> GetEnumerator()
        {
            return Items.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class HiddenItems: RelatedActivityList
    {
        
        public HiddenItems(ulong sourceId, string sourceType):base(sourceId,sourceType)
        { 
        }

        protected override ulong[] GetFromDB()
        {
            return HiddenActivities.GetForUser(_sourceId).Select(a => (ulong)a.hide_resource_id).ToArray();
        }

        protected override string Label => "hidden";

        public void Hide(ulong actionId)
        {
            Add(actionId);
        }

        public void UnHide(ulong actionId)
        {
            Remove(actionId);
        }

        public bool IsHidden(ulong actionId)
        {
            return Exists(actionId);
        }
    }

    public class SavedItems : RelatedActivityList
    {
        public SavedItems(ulong sourceId, string sourceType) : base(sourceId, sourceType)
        {
        }

        protected override ulong[] GetFromDB()
        {
            return SavedActivities.GetForUser((int)_sourceId).Select(a => (ulong)a.action_id).ToArray();
        }

        protected override string Label => "saved";

        public void Save(ulong actionId)
        {
            Add(actionId);
        }

        public void Unsave(ulong actionId)
        {
            Remove(actionId);
        }

        public bool IsSaved(ulong actionId)
        {
            return Exists(actionId);
        }
    }
}