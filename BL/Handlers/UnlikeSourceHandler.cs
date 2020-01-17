using System;
using System.Collections.Generic;
using System.Text;
using KoobecaFeedController.BL.Request;
using KoobecaFeedController.DAL.Adapters;
using KoobecaFeedController.DAL.Models;

namespace KoobecaFeedController.BL.Handlers
{
    public class UnlikeSourceHandler : ActivityHandler
    {
        public UnlikeSourceHandler(uint userId, RequestParams reqParams) : base(userId, reqParams)
        {
        }

        public override void Execute()
        {
            if(activity.EffectiveSubjetType == "sitepage_page")
            {
                var likedSrc = UsersNetworks.Instance.GetLikedSources(userId).GetSource(activity.EffectiveSubjectId, activity.EffectiveSubjetType);
                if (likedSrc != null)
                {
                    //update the cache
                    likedSrc.AffinityLevel = (byte)(likedSrc.AffinityLevel / 2); //reduce it by 50%
                    //update the DB
                    SourceAffinities.UpdateAffinity(userId, activity.EffectiveSubjectId, activity.EffectiveSubjetType, likedSrc.AffinityLevel);
                }
                else {
                    //if we never liked it before and we don't like it, we will set it with affinity 0 to make sure we don't get it
                    SourceAffinities.Add(new SourceAffinity()
                    {
                        affinity = 0,
                        user_id = this.userId,
                        source_id = activity.EffectiveSubjectId,
                        source_type = activity.EffectiveSubjetType,
                        date = DateTime.Now
                    });
                    UsersNetworks.Instance.GetLikedSources(userId).AddSource(activity.EffectiveSubjectId, activity.EffectiveSubjetType, 0);
                }
            }
            else
            {
                Logger.Instance.Warn($"can't unlike this source.subject type is {activity.EffectiveSubjetType}");
            }
        }
    }
}
