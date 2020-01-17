using System;
using System.Collections.Generic;
using System.Text;
using KoobecaFeedController.BL.Request;

namespace KoobecaFeedController.BL.Handlers
{
    public class UpdateSharableHandler : ActivityHandler
    {
        public UpdateSharableHandler(uint userId, RequestParams reqParams) : base(userId, reqParams)
        {
        }

        public override void Execute()
        {
            activity.RawActivity.shareable = !activity.RawActivity.shareable;
            UpdateActionInDB(activity.RawActivity);
            Response = activity.RawActivity.shareable ? "1" : "0";
        }
    }
}
