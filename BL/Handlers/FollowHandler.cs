using System;
using System.Collections.Generic;
using System.Text;
using KoobecaFeedController.BL.Request;
using KoobecaFeedController.DAL.Adapters;
using KoobecaFeedController.DAL.Models;

namespace KoobecaFeedController.BL.Handlers
{
    public class FollowHandler : ActivityHandler
    {
        private ulong sourceId;
        private string srcType;// = "sitepage_page";

        public FollowHandler(uint userId, RequestParams reqParams, Dictionary<string, object> additionalParams) : base(userId, reqParams)
        {
            if (additionalParams.ContainsKey("src_id"))
            {
                sourceId = ulong.Parse((string)additionalParams["src_id"]);
            }
            if (additionalParams.ContainsKey("src_type"))
            {
                srcType = (string)additionalParams["src_type"];
            }
        }

        public override void Execute()
        {
            Logger.Instance.Debug($"{srcType} id is {sourceId}");
            //get the page
            IFollowedSource followedSrc = null;
            if(srcType.Contains("page"))
                followedSrc = Pages.GetById((uint)sourceId);
            else if (srcType.Contains("group"))
                followedSrc = Groups.GetById((uint)sourceId);

            var followRecord = FollowedItems.GetFollow(userId, sourceId,srcType);
            var followedSources = UsersNetworks.Instance.GetFollowedSources(userId);
            if (followRecord != null)
            {
                //unfollow
                Logger.Instance.Debug("unfollowing");
                followedSources.RemoveSource(sourceId, srcType);
                FollowedItems.RemoveFollow(userId, sourceId,srcType);
                followedSrc.DecreaseFollow();
            }
            else
            {
                //follow
                Logger.Instance.Debug("following");
                followedSources.AddSource(sourceId, srcType);
                FollowedItems.Add(new DAL.Models.FollowedItem()
                {
                    poster_id = userId,
                    poster_type = "user",
                    resource_id = (uint)sourceId,
                    resource_type = srcType,
                    creation_date = DateTime.Now
                });
                followedSrc.IncreasFollow();
            }
            //update the page in DB
            if(followedSrc is Page)
                Pages.UpdateFollowCount((uint)sourceId, ((Page)followedSrc).follow_count);
            else if (followedSrc is Group)
                Groups.UpdateFollowCount((uint)sourceId, ((Group)followedSrc).follow_count);
        }
    }
}
