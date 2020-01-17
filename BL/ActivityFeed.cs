using System;
using System.Collections.Generic;
using System.Linq;
using KoobecaFeedController.BL.Handlers;
using KoobecaFeedController.DAL.Adapters;
using KoobecaFeedController.DAL.Models;

namespace KoobecaFeedController.BL {
    public class ActivityFeed {
        private readonly object _lock = new object();
        private List<ulong> _feed;
        private readonly ulong _sourceId;
        private readonly string _sourceType;
        private bool canAppend = true; // if there are no more older activities in DB for this it will be false


        public ActivityFeed(ulong sourceId, string sourceType) {
            _sourceId = sourceId;
            _sourceType = sourceType;
        }
        
        public List<ulong> Feed {
            get {
                lock (_lock) {
                    if (_feed == null) {
                        //Logger.Instance.Debug($"initializing feed of {_sourceType}:{_sourceId}");
                        InitFeed();
                    }
                }
                return _feed;
            }
        }

        private DateTime _MinimumDate = DateTime.MinValue;
        public DateTime MinimumDate
        {
            get
            {
                var feed = Feed.FirstOrDefault(); //taht's jsut to initialize the feed
                return _MinimumDate;
            }
            internal set
            {
                _MinimumDate = value;
            }
        }

        private void InitFeed() {
            if(_sourceId == 339)
            {
                Logger.Instance.Debug("initiating pageadmin feed");
            }
            AppendFeed(DateTime.Now,50);
        }

        public bool AppendFeed(DateTime endTime,int count)
        {
            if (!canAppend) return false;

            lock (_lock)
            {
                var newfeed = new List<ulong>();
                
                var fetchedActivities = ActivityActions.GetBySubjectOrObjectId((uint)_sourceId, _sourceType, endTime, count);
                if (_sourceId == 339)
                {
                    Logger.Instance.Debug($"got {fetchedActivities.Count()} ativities from DB before date {endTime}");
                }

                if (fetchedActivities.Count() < count) canAppend = false; //there is no point in trying to fetch more

                foreach (var rawActivity in fetchedActivities.Reverse())
                {
                    //add it to the cache (if needed) and the id to the list
                    ActivityCache.Instance.Set(new Activity(rawActivity));
                    newfeed.Add(rawActivity.action_id);
                }

                if (newfeed.Count > 0)
                {
                    //update minDate
                    MinimumDate = ActivityCache.Instance.Get(newfeed.Min(id=>id)).CreationDate;
                }

                if (_feed == null)
                {
                    //first time
                    _feed = newfeed;
                }
                else
                {
                    //we are appending to existing feed
                    newfeed.AddRange(_feed);
                    _feed = newfeed;
                }

                return canAppend;
            }
        }

        

        public void AdvanceActivity(Activity a,bool advancePeer = true) {
            lock (_lock) {
                if (_feed == null) return;
                _feed.Remove(a.ActivityId);
                _feed.Add(a.ActivityId);
            }
            if (advancePeer)
            {
                SourceData peer = GetPeer(a.RawActivity);
                peer?.ActivityFeed.AdvanceActivity(a, false);
            }
        }

        public void AddActivity(Activity a,bool addToPeer = true) {
            lock (_lock) {
                if (_feed == null) return;
                _feed.Add(a.ActivityId);
            }
            if (addToPeer)
            {
                SourceData peer = GetPeer(a.RawActivity);
                peer?.ActivityFeed.AddActivity(a,false);
            }
        }

        public void RemoveActivity(uint actionId, bool deleteAll)
        {
            var activity = ActivityCache.Instance.Get(actionId);
            if (activity != null)
            {
                RemoveActivity(activity, deleteAll);
            }
        }

        public void RemoveActivity(Activity a, bool deletetOnPeer) {

            lock (_lock)
            {
                if (_feed == null) return;
                _feed.Remove(a.ActivityId);

                if (deletetOnPeer)
                {
                    //see if needed to be removed to the peer
                    SourceData peer = GetPeer(a.RawActivity);
                    peer?.ActivityFeed.RemoveActivity(a, false);
                }
            }
        }

        private SourceData GetPeer(ActivityAction rawActivity)
        {
            ulong peerId = rawActivity.subject_id == _sourceId
                             ? rawActivity.object_id
                             : rawActivity.subject_id;
            var peerType = rawActivity.subject_type == _sourceType
                ? rawActivity.object_type
                : rawActivity.subject_type;
            if (peerId == _sourceId && peerType == _sourceType) return null;//no real peer
            return SourcesMap.Instance.GetSource(peerId, peerType);
        }
    }
}