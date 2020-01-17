using System;
using System.Collections.Generic;
using System.Text;
using KoobecaFeedController.BL.Request;
using KoobecaFeedController.DAL.Adapters;

namespace KoobecaFeedController.BL.Handlers
{
    public class UpdateCommentableHandler : ActivityHandler
    {
        public UpdateCommentableHandler(uint userId, RequestParams reqParams) : base(userId, reqParams)
        {
        }

        public override void Execute()
        {
            activity.RawActivity.commentable = !activity.RawActivity.commentable;
            UpdateActionInDB(activity.RawActivity);
            Response = activity.RawActivity.commentable ? "1" : "0";
        }
    }
}
