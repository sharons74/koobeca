using KoobecaFeedController.BL.FeedGenerators;
using KoobecaFeedController.BL.Request;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KoobecaFeedController.BL.Handlers
{
    public class GetFeedHandler : ActivityHandler
    {
        public GetFeedHandler(uint userId, RequestParams reqParams) : base(userId, reqParams)
        {}

        public override void Execute()
        {
            FeedGenerator generator = null;

            //if(string.IsNullOrEmpty(reqParams.filter_type))
            //{
            //    //in case no explicit data for feed generator is given
            //    var viewer = SourcesMap.Instance.GetSource(userId, "user");
            //    reqParams.filter_type = viewer.LastFeedFilterType;
            //    Logger.Instance.Debug($"taking filter type:{reqParams.filter_type} from viewer memory");
            //}

            if (reqParams.action_id != 0)
            {
                generator = new SingleActivityGenerator(reqParams);
            }
            else if (reqParams.subject_id != 0)
            {
                generator = new SingleSourceFeedGenerator(reqParams);
            }
            else if(reqParams.filter_type == "membership")
            {
                generator = new  FriendsFeedGenerator(reqParams);
            }
            else if (reqParams.filter_type == "sitegroup")
            {
                generator = new GroupsFeedGenerator(reqParams);
            }
            else if (reqParams.filter_type == "sitepage")
            {
                generator = new PagesFeedGenerator(reqParams);
            }
            else if (reqParams.filter_type == "user_saved")
            {
                generator = new SavedFeedGenerator(reqParams);
            }
            else if (reqParams.filter_type == "hidden_post")
            {
                generator = new HiddenFeedGenerator(reqParams);
            }
            else if (reqParams.filter_type.ToLower() == "all")
            {
                generator = new MainFeedGenerator(reqParams);
            }
            else if (!string.IsNullOrEmpty(reqParams.filter_type))
            {
                //code for different filters
                reqParams.action_id = 11432;//oops
                generator = new SingleActivityGenerator(reqParams);
                
            }

            Response = generator.GetFeedJson();

            File.WriteAllText("kfcfeed.txt", Response);
        }

        protected override bool IsAuthorized
        {
            get
            {
                return true; //currently we do not check authorization on getting feed, the check is done on each item specifically
            }
        }
    }
}
