using KoobecaFeedController.BL.Rendering;
using KoobecaFeedController.BL.Request;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace KoobecaFeedController.BL.FeedGenerators
{
    public class FeedGenerator
    {
        protected int feedSizeLimit = 50; //default;
        protected ulong feedMaxId;
        protected uint viewerId;
        protected SourceData viewer;
        protected SourceData timeSliceSource;
        protected Dictionary<ulong, CombinedActivity> combinedActivities = new Dictionary<ulong, CombinedActivity>();
        protected NetworkMemberFilter memberFilter;
        protected FeedFilter feedFilter;
        protected bool noFeed; //in case we get minid in request

        public FeedGenerator(RequestParams request)
        {
            if (request.limit > 0)
            {
                Logger.Instance.Debug($"set feed size limit to {request.limit}");
                feedSizeLimit = request.limit;
            }

            IsIOS = string.IsNullOrEmpty(request._IOS_VERSION) ? false : true;

            viewerId = request.viewerId;
            viewer = SourcesMap.Instance.GetSource(request.viewerId, "user");
            SetFeedMode(request.maxid, request.minid);
            
            //if (!string.IsNullOrEmpty(request.filter_type))
            //{
            //    Logger.Instance.Debug($"storing filter type:{request.filter_type} for user {viewer.SourceId}");
            //    viewer.LastFeedFilterType = request.filter_type; //save it for unknown feed generation requests
            //}
            timeSliceSource = viewer;
            memberFilter = CreateMemberFilter();
            feedFilter = CreateFeedFilter();
    }

        private void SetFeedMode(int maxid, int minid)
        {
            if (maxid == 0 && minid == 0)
            {
                //need brand new feed
                Logger.Instance.Debug($"getting brand new feed");
                feedMaxId = ulong.MaxValue; //no filter on that
                viewer.LastMinimalIdInFeed = ulong.MaxValue; //clean the memmory of the viewer
            }
            else if (maxid > 0)
            {
                // we need to take feed from last id
                feedMaxId = viewer.LastMinimalIdInFeed;
                Logger.Instance.Debug($"appending to feed feed");
            }
            else if (minid > 0)
            {
                Logger.Instance.Debug($"got minid={minid}");
                noFeed = true; //in this case we return nothing
            }
        }

        public string GetFeedJson()
        {
            if (noFeed)
            {
                Logger.Instance.Debug("retuning no feed");
                return string.Empty;
            }
            
            return Serialize(GetFeed());
        }

        /// <summary>
        /// Return the desired feed as collection of activity objects wrapper in combined activity containers
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<CombinedActivity> GetFeed()
        { 
            var sourceList = GetFeedSources();

            Logger.Instance.Debug($"number of sources is {sourceList.Length}");
            if(sourceList.Length == 0)
            {
                return new List<CombinedActivity>();
            }


            int maxLoops = 100;//we won't do more than 100 loops //(int)(100 / TimeSlice); //we wont go more than 100 days back
            if (maxLoops == 0) maxLoops = 1; //at least 1 1
            int loopCount = 0;
            Logger.Instance.Debug($"current timeslice is {TimeSlice} days, from source {timeSliceSource.SourceId}");
            int initialCount = combinedActivities.Count;
            double actualTimeSlice = TimeSlice;
            while (combinedActivities.Count < feedSizeLimit && loopCount++ < maxLoops)
            {
                if(feedFilter.EndDate < LocalTime.StartTime)
                {
                    //there are no feeds in the system before this start time
                    Logger.Instance.Debug($"No feeds in the system before {LocalTime.StartTime}");
                    break;
                    //TODO also check for each source it's trart time when getting feed
                }

                int beforeIteration = combinedActivities.Count;
                //go over all sources
                foreach (var source in sourceList)
                {
                    //get source activities filtered
                    foreach (var activity in source.GetFilteredFeed(feedFilter))
                    {
                        AddActivityToCombined(activity);
                    }
                }
                Logger.Instance.Debug($"combined count after {loopCount} loops is {combinedActivities.Count}");

                if(beforeIteration == combinedActivities.Count)
                {
                    //no increase in activities, try begger time slice
                    
                    var newActualTimeSlice = actualTimeSlice * 1.1;
                    if (newActualTimeSlice < 10)
                        actualTimeSlice = newActualTimeSlice; //we don't want bigger timeslices in loop inflation
                }
                else
                {
                    actualTimeSlice = TimeSlice;
                }
                //fetch earlier items, go one time slice back
                feedFilter.EndDate = feedFilter.StartDate;
                //Logger.Instance.Debug($"subtracting {actualTimeSlice} days from start date {feedFilter.StartDate}");
                feedFilter.StartDate = feedFilter.EndDate.AddDays(-actualTimeSlice);
                //Logger.Instance.Debug($"filter updater to {feedFilter.StartDate} --- {feedFilter.EndDate}");
            }
            Logger.Instance.Debug($"got {combinedActivities.Count} combined activities after {loopCount} loops");
            if(loopCount > 0 && TimeSlice * loopCount <= 101) //we do not allow time slice more than 100 days
                UpdateTimeSlice(loopCount, combinedActivities.Count - initialCount, feedSizeLimit);
            //sort the combined list by modification date and return it
            return combinedActivities.Select(p => p.Value).OrderByDescending(c => c.MaxId).Take(feedSizeLimit);
            //return combinedActivities.Select(p => p.Value).OrderByDescending(c => c.EffctiveDate).Take(feedSizeLimit);
        }


        private void UpdateTimeSlice(int loopCount,int actualCount,int plannedCount)
        {
            var currTimeSlice = TimeSlice;
            Logger.Instance.Debug($"Current time slice is {currTimeSlice}");

            if(loopCount > 1)
            {  
                TimeSlice = TimeSlice * loopCount;
                Logger.Instance.Debug($"Updating time slice to {TimeSlice} days");
            }
            else if(loopCount == 1 && actualCount/(plannedCount * 1.0) > 1.2)
            {
                Logger.Instance.Debug($"planned {plannedCount} combined and got {actualCount}");
                TimeSlice = TimeSlice / (actualCount / (plannedCount * 1.0));
                Logger.Instance.Debug($"Updating time slice to {TimeSlice} days");
            }
            else
            {
                Logger.Instance.Debug($"Got {actualCount} combined with time slice of {TimeSlice} days");
            }
        }

        protected virtual IEnumerable<CombinedActivity> GetCombinedList(List<Activity> rawFeed)
        {
            throw new NotImplementedException();
        }

        private string Serialize(IEnumerable<CombinedActivity> activities)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var activityList = activities.Where(a=>a.HasActivities).ToList();

            sw.Stop();
            Logger.Instance.Debug($"Getting combined activity list of {activityList.Count()} took {sw.ElapsedMilliseconds} ms");
            sw.Reset();
            sw.Start();
            var feed = activityList.Select(a => FeedWrapper.Create(a, viewerId, "user",IsIOS)).Where(w => w != null).ToArray();
            sw.Stop();
            Logger.Instance.Debug($"creating feedwrappers took {sw.ElapsedMilliseconds} ms");
            ulong minId = 0;
            ulong maxId = 0;
            if (activityList.Count > 0)
            {
                minId = GetMinId(activityList);
                maxId = GetMaxId(activityList);
                Logger.Instance.Debug($"minid is:{minId} maxid is:{maxId}");
                if((maxId - minId) > 1000)
                {
                    //too big gap
                    activityList.ForEach(ca => ca.PrintAll());
                }

                viewer.LastMinimalIdInFeed = minId; // update it for the next fetch
            }

            return JsonConvert.SerializeObject(new ResponseBody(feed,minId,maxId,feedSizeLimit), Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });
        }

        private ulong GetMinId(List<CombinedActivity> activities)
        {
            if (activities.Count == 0) return 0;

            var minId = activities.OrderBy(a => a.MinId).First().MinId;
            if(minId == ulong.MaxValue)
            {
                Logger.Instance.Debug($"first search for minId resulted in {minId}, taking the minimal of max ids");
                minId = activities.OrderBy(a => a.MaxId).First().MaxId;
            }
            return minId;
                 
        }

        private ulong GetMaxId(List<CombinedActivity> activities)
        {
            if (activities.Count == 0) return 0;

            return activities.OrderBy(a => a.MaxId).Last().MaxId;
        }

        private FeedFilter CreateFeedFilter()
        {
            Logger.Instance.Debug($"Setting maximal fetch to {feedMaxId}");

            return new FeedFilter()
            {
                EndDate = DateTime.Now,
                StartDate = DateTime.Now.AddDays(-TimeSlice),
                Viewer = viewer,
                MaxId = feedMaxId
            };
        }

        protected virtual NetworkMemberFilter CreateMemberFilter()
        {
            //TODO: in the future we want to filter by affinity
            return new NetworkMemberFilter();
        }

        protected virtual SourceData[] GetFeedSources()
        {
            throw new NotImplementedException();
        }

        protected void AddActivityToCombined(Activity activity)
        {
            try
            {
                if (activity.IsPrimary && !combinedActivities.ContainsKey(activity.ActivityId))
                {
                    //primary activity first time
                    var newCombined = new CombinedActivity { PrimeActivity = activity };
                    combinedActivities[activity.ActivityId] = newCombined;
                }
                else if (activity.InnerActivity != null)
                {
                    //reference activity with inner activity
                    if (!combinedActivities.ContainsKey(activity.InnerActivityId))
                    {
                        combinedActivities[activity.InnerActivityId] = new CombinedActivity { PrimeActivity = activity.InnerActivity };
                    }
                    AddRefActivity(activity);
                }
            }
            catch(Exception e)
            {
                Logger.Instance.Error($"failed to add activity {activity?.ActivityId} to combined activity. msg:{e.Message}");
                Logger.Instance.Debug(e.ToString());
            }
        }

        private void AddRefActivity(Activity activity)
        {
            if (activity.IsLikeOrReact && activity.SubjectId == viewerId && activity.InnerActivity.SubjectId == viewerId)
            {
                //just update the min max even though we wont show this ref
                combinedActivities[activity.InnerActivityId].UpdateMaxMinId(activity);
            }
            else
            {
                combinedActivities[activity.InnerActivityId].RefActivities[activity.OwnerId] = activity;
                combinedActivities[activity.InnerActivityId].UpdateMaxMinId(activity);
            }
        }


        /// <summary>
        /// The time slice to take activities from all members in each feed generator
        /// </summary>
        protected double TimeSlice
        {
            get
            {
                var key = this.GetType().Name;
                if (!timeSliceSource.FeedTimeSlices.ContainsKey(key)){
                    timeSliceSource.FeedTimeSlices[key] = 1; //initial setting one day by default
                }
                return timeSliceSource.FeedTimeSlices[key]; 
            }
            set
            {
                timeSliceSource.FeedTimeSlices[this.GetType().Name] = value;
            }
        }

        protected bool IsIOS { get; set; }

    }
}
