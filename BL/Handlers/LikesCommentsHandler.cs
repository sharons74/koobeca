using System;
using System.Collections.Generic;
using System.Text;
using KoobecaFeedController.BL.Rendering.Comments;
using KoobecaFeedController.BL.Request;
using Newtonsoft.Json;

namespace KoobecaFeedController.BL.Handlers
{
    public class LikesCommentsHandler : ActivityHandler
    {
        public LikesCommentsHandler(uint userId, RequestParams reqParams) : base(userId, reqParams)
        {
        }

        public override void Execute()
        {
            if(reqParams.action_id > 0)
            {
                bool viewComments = reqParams.viewAllComments == "1";
                var res = LikesCommentsBody.Create(activity,userId,viewComments,reqParams.comment_id,reqParams.ios);

                Response = JsonConvert.SerializeObject(res);
            }
        }
    }
}
