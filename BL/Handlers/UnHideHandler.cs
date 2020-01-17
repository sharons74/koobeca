using System;
using System.Collections.Generic;
using System.Text;
using KoobecaFeedController.BL.Request;
using KoobecaFeedController.DAL.Adapters;

namespace KoobecaFeedController.BL.Handlers
{
    public class UnHideHandler : ActivityHandler
    {
        public UnHideHandler(uint userId, RequestParams reqParams) : base(userId, reqParams)
        {
        }

        public override void Execute()
        {
            Logger.Instance.Debug($"unhiding activity {reqParams.action_id} from user {userId}");
            HiddenActivities.Remove(userId, (uint)reqParams.action_id);
            userSource.HidenItems.UnHide(reqParams.action_id);

            RelikePublisher();
            //GetFeedHandler getFeedHandler = new GetFeedHandler(userId, reqParams);
            //getFeedHandler.Execute();
            //Response = getFeedHandler.Response;
        }

        private void RelikePublisher()
        {
            if (activity.EffectiveSubjetType == "sitepage_page")
            {
                var likedSrc = UsersNetworks.Instance.GetLikedSources(userId).GetSource(activity.EffectiveSubjectId, activity.EffectiveSubjetType);
                if (likedSrc != null)
                {
                    //update the cache
                    if (likedSrc.AffinityLevel == 0)
                    {
                        //remove it
                        UsersNetworks.Instance.GetLikedSources(userId).RemoveSource(activity.EffectiveSubjectId, activity.EffectiveSubjetType);
                        SourceAffinities.RemoveAffinity(userId, activity.EffectiveSubjectId, activity.EffectiveSubjetType);
                    }
                    else
                    {
                        likedSrc.AffinityLevel = (byte)(likedSrc.AffinityLevel * 2); //double it to undo the reduction
                        //update the DB
                        SourceAffinities.UpdateAffinity(userId, activity.EffectiveSubjectId, activity.EffectiveSubjetType, likedSrc.AffinityLevel);
                    }
                    
                }
            }
            else
            {
                Logger.Instance.Debug($"can't relike this source.subject type is {activity.EffectiveSubjetType}");
            }
        }
    }
}
