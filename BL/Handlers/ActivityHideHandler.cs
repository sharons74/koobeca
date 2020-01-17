using KoobecaFeedController.BL.Request;

namespace KoobecaFeedController.BL.Handlers
{
    internal class ActivityHideHandler : ActivityHandler
    {
        public ActivityHideHandler(uint userId, RequestParams reqParams) : base(userId, reqParams)
        {
        }

        public override void Execute()
        {
            //Engine_Api::_()->getApi('Siteapi_Core', 'activity')->markNotificationsAsRead($viewer); TODO
            Response = "";
        }
    }
}