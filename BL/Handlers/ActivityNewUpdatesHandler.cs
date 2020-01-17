using KoobecaFeedController.BL.Request;
using Newtonsoft.Json;

namespace KoobecaFeedController.BL.Handlers
{
    internal class ActivityNewUpdatesHandler : ActivityHandler
    {
        public ActivityNewUpdatesHandler(uint userId, RequestParams reqParams) : base(userId, reqParams)
        {
        }

        public override void Execute()
        {
            var ret = new {
                notifications = 1,
                friend_requests = 2,
                messages = 3
            };
            Response = JsonConvert.SerializeObject(ret);
        }
    }
}