using KoobecaFeedController.BL.Request;
using Newtonsoft.Json;

namespace KoobecaFeedController.BL.Handlers
{
    internal class CoreGetSettingsHandler : ActivityHandler
    {
        public CoreGetSettingsHandler(uint userId, RequestParams reqParams) : base(userId, reqParams)
        {
        }

        public override void Execute()
        {
            object ret = new 
            {
                sitequicksignup = new
                {
                    isQuickSignUp = true,
                    isAllowTitle = true,
                    isAllowDescription = true,
                    isAllowFieldDescription = true,
                    isAllowConfirmEmail = false,
                    isAllowConfirmPassword = true,
                    isEnabledSubscription = false,
                    isEnabledPopUp = true
                }
            };
            Response = JsonConvert.SerializeObject(ret);
        }
    }
}