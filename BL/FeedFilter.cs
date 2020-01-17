using System;

namespace KoobecaFeedController.BL
{
    public class FeedFilter
    {
        public DateTime EndDate { get; internal set; }
        public DateTime StartDate { get; internal set; }
        public SourceData Viewer { get; set; }
        public ulong MaxId { get; set; }
        public bool Match(Activity activity,out bool toYoung)
        {
            toYoung = false; //initially
            try
            {
                if (activity == null)
                {
                    FailedActivities.Add(activity.ActivityId);
                    return false;
                }

                if (MaxId > 0 && activity.ActivityId >= MaxId) return false; //id need to be lower then MaxId

                
                //check that this is not faulted activity
                if (FailedActivities.Contains(activity.ActivityId)) return false;
                

                Activity refActivity = activity.IsPrimaryOrShare ? activity : activity.InnerActivity;

                if (refActivity == null)
                {
                    Logger.Instance.Error($"activity {activity.ActivityId} is of type {activity.Type} but has no inner activity");
                    
                    ActivityRemover.Instance.AddActivity(activity.ActivityId); //TODO get remover operational
                    return false;
                }

                if (!(refActivity.CreationDate > StartDate && refActivity.CreationDate < EndDate))
                {
                    if (refActivity.CreationDate < StartDate) toYoung = true;
                    //Logger.Instance.Error($"activity {refActivity.ActivityId} is of type {refActivity.Type} filtered because of creation date {refActivity.CreationDate}");
                    return false;
                }
                if (refActivity.HiddenFor(Viewer))
                {
                    //Logger.Instance.Error($"activity {refActivity.ActivityId} is of type {refActivity.Type} filtered because it is hidden to user {Viewer.SourceId}");
                    return false;
                }
                if (refActivity.IsPrimaryOrShare && !PrivacyMatch(refActivity))
                {
                    //Logger.Instance.Error($"activity {refActivity.ActivityId} is of type {refActivity.Type} filtered because it is primary or share and there is no privacy match for viewing");
                    return false;
                }

                return true;
            }
            catch(Exception e)
            {
                Logger.Instance.Error($"filter faild on activity {activity.ActivityId}");
                Logger.Instance.Debug(e.ToString());
                return false;
            }
        }

        private bool PrivacyMatch(Activity a)
        {
            //TODO need to revise this
            var authorization = a.GetAuthorization(Viewer.SourceId, "user");

            return (authorization & (ushort)AuthorizationFlags.View) > 0;
        }
    }
}