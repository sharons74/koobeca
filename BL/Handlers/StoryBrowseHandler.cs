using KoobecaFeedController.BL.Request;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace KoobecaFeedController.BL.Handlers
{
    internal class StoryBrowseHandler : ActivityHandler
    {
        public StoryBrowseHandler(uint userId, RequestParams reqParams) : base(userId, reqParams)
        {
        }

        public override void Execute()
        {
            Story s = new Story();
            s.owner_title = "Your Story";
            s.owner_image = "http://beta.koobeca.com//application/modules/User/externals/images/nophoto_user_thumb_profile.png";
            s.owner_image_normal = "http://beta.koobeca.com//application/modules/User/externals/images/nophoto_user_thumb_profile.png";
            s.owner_image_profile = "http://beta.koobeca.com//application/modules/User/externals/images/nophoto_user_thumb_profile.png";
            s.owner_image_icon = "http://beta.koobeca.com//application/modules/User/externals/images/nophoto_user_thumb_icon.png";

            var ret = new {
                totalItemCount = 0,
                muteStoryCount = 0,
                response = new Story[] { s }
            };

            Response = JsonConvert.SerializeObject(ret);
        }
    }
}