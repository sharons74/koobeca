using System;
using System.Collections.Generic;
using System.Text;
using KoobecaFeedController.BL.Rendering;
using KoobecaFeedController.BL.Request;
using KoobecaFeedController.DAL.Adapters;
using KoobecaFeedController.DAL.Models;
using Newtonsoft.Json;

namespace KoobecaFeedController.BL.Handlers
{
    public class HideHandler : ActivityHandler
    {
        public HideHandler(uint userId, RequestParams reqParams) : base(userId, reqParams)
        {
        }

        public override void Execute()
        {
            Logger.Instance.Debug($"Hiding activity {reqParams.action_id} from user {userId}");

            var hidden = HiddenActivities.Get(userId, (uint)reqParams.action_id);
            if(hidden != null)
            {
                Logger.Instance.Warn($"activity {reqParams.action_id} is already hidden for user {userId}");
                return;
            }
            hidden = new DAL.Models.HiddenActivity() { user_id = userId , hide_resource_id = (uint)reqParams.action_id};
            HiddenActivities.Add(hidden);
            userSource.HidenItems.Hide(reqParams.action_id);
            FeedWrapper.Menus.Item item = new FeedWrapper.Menus.Item()
            {
                name = "undo",
                //label = "This story is now hidden from your Activity Feed.",
                url = "advancedactivity/feeds/un-hide-item"
            };
            item.label = $"you will see less things from this publisher";
            item.urlParams = new FeedWrapper.Menus.Item.UrlParams()
            {
                type = "activity_action",
                //id = (uint)reqParams.action_id,
                action_id = (uint)reqParams.action_id
            };

            UnlikePublisher();

            Response = JsonConvert.SerializeObject(new { undo = item });
            Logger.Instance.Debug($"response: {Response}");
        }

        private void UnlikePublisher()
        {
            if (activity.EffectiveSubjetType == "sitepage_page")
            {
                var likedSrc = UsersNetworks.Instance.GetLikedSources(userId).GetSource(activity.EffectiveSubjectId, activity.EffectiveSubjetType);
                if (likedSrc != null)
                {
                    //update the cache
                    likedSrc.AffinityLevel = (byte)(likedSrc.AffinityLevel / 2); //reduce it by 50%
                    if (likedSrc.AffinityLevel == 0) likedSrc.AffinityLevel = 1; //we don't go to 0
                    //update the DB
                    SourceAffinities.UpdateAffinity(userId, activity.EffectiveSubjectId, activity.EffectiveSubjetType, likedSrc.AffinityLevel);
                }
                else
                {
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
                Logger.Instance.Debug($"can't unlike this source.subject type is {activity.EffectiveSubjetType}");
            }
        }
    }
}
