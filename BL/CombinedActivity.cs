using System;
using System.Collections.Generic;
using System.Linq;

namespace KoobecaFeedController.BL {
    public class CombinedActivity {
        private ulong _minId = ulong.MaxValue;
        private ulong _maxId = 0;

        public Activity PrimeActivity { get; set; }

        public Dictionary<ulong, Activity> RefActivities { get; } = new Dictionary<ulong, Activity>();
        public ulong FirstReactor { get; set; }
        //public DateTime EffectiveDate {
        //    get
        //    {
        //        if (RefActivities.Count > 0)
        //        {
        //            return RefActivities.First().Value.ModifyDate;
        //        }
        //        else
        //        {
        //            return PrimeActivity.ModifyDate;
        //        }
        //    }
        //}
        public ulong MinId {
            get
            {
                return (RefActivities.Count > 0) ? _minId : PrimeActivity.ActivityId;
                //if (RefActivities.Count > 0) {
                //    return _minId;
                //}
                //else {
                //    //return the primary id only if it was never modified. that's because we can have some old items
                //    //that where modified by activities that are not included in our feed 
                //    return PrimeActivity.RawActivity.date == PrimeActivity.RawActivity.modified_date?PrimeActivity.ActivityId:ulong.MaxValue;
                //}
            }
        }

        public ulong MaxId
        {
            get
            {
                return (RefActivities.Count > 0) ? _maxId : PrimeActivity.ActivityId;
            }
        }

        public DateTime EffctiveDate
        {
            get
            {
                var list = RefActivities.Values.ToList();
                list.Add(PrimeActivity);
                return list.Max(i => i.CreationDate); //return the most modified of all
            }
        }

        public bool HasActivities {
            get
            {
                return RefActivities.Count > 0 || PrimeActivity != null; //look for at least 1 activity
            }
         }

        public void UpdateMaxMinId(Activity activity)
        {
            //done only when we add ref activity
            _maxId = Math.Max(_maxId, activity.ActivityId);
            _minId = Math.Min(_minId, activity.ActivityId);
        }

        public void PrintAll()
        {
            Logger.Instance.Debug("Combined Activity:");
            Logger.Instance.Debug($"Primary id={PrimeActivity.ActivityId} creation={PrimeActivity.RawActivity.date} modifiy={PrimeActivity.RawActivity.modified_date} type {PrimeActivity.Type}");
            foreach (Activity a in RefActivities.Values)
            {
                Logger.Instance.Debug($"id={a.ActivityId} creation={a.RawActivity.date} modifiy={a.RawActivity.modified_date} type {a.Type}");
            }
        }

        public List<Activity> Shares
        {
            get
            {
                return RefActivities.Values.Where(a => a.IsShare).ToList();
            }
        }

        public List<Activity> Comments
        {
            get
            {
                return RefActivities.Values.Where(a => a.Type == ActivityType.Comment).ToList();
            }
        }

    }
}