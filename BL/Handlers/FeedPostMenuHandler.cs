using System;
using System.Collections.Generic;
using System.Text;
using KoobecaFeedController.BL.Request;
using Newtonsoft.Json;

namespace KoobecaFeedController.BL.Handlers
{
    public class FeedPostMenuHandler : ActivityHandler
    {
        public FeedPostMenuHandler(uint userId, RequestParams reqParams) : base(userId, reqParams)
        {
        }

        public override void Execute()
        {

            var resObj = new
            {
                feed_post_menu = new
                {
                    status = 1,
                    withtags = 1,
                    emotions = 1,
                    photo = 1,
                    checkin = 1,
                    video = 1,
                    music = 0,
                    link = 1,
                    userprivacy = new
                    {
                        everyone = "Everyone",
                        onlym = "Only me",
                        friends = "Friends"
                    }
                }
            };


            Response = JsonConvert.SerializeObject(resObj);
        } 
    }
}
