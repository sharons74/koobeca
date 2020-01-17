using KoobecaFeedController.BL.Request;

namespace KoobecaFeedController.BL.Handlers
{
    internal class GetVersionHandler : ActivityHandler
    {
        public GetVersionHandler(uint userId, RequestParams reqParams) : base(userId, reqParams)
        {
        }

        public override void Execute()
        {
            Response = @"{""isForceUpgrade"":true,""latestVersion"":"""",""description"":"""",""isPopUpEnabled"":""0""}";
        }
    }
}