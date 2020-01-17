using KoobecaFeedController.BL.Request;

namespace KoobecaFeedController.BL.Handlers
{
    internal class ErrorPageContentHandler : ActivityHandler
    {
        public ErrorPageContentHandler(uint userId, RequestParams reqParams) : base(userId, reqParams)
        {
        }

        public override void Execute()
        {
            Response = "";
        }
    }
}