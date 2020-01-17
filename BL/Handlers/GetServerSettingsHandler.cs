using KoobecaFeedController.BL.Request;
using Newtonsoft.Json;

namespace KoobecaFeedController.BL.Handlers
{
    internal class GetServerSettingsHandler : ActivityHandler
    {
        public GetServerSettingsHandler(uint userId, RequestParams reqParams) : base(userId, reqParams)
        {
        }

        public override void Execute()
        {
            //{"rest_space":-1,"upload_max_size_limit":"32","upload_max_size":"32M"}
            var ret = new
            {
                rest_space = -1,
                upload_max_size_limit = 32,
                upload_max_size = "32M"
            };
            Response = JsonConvert.SerializeObject(ret);
        }
    }
}