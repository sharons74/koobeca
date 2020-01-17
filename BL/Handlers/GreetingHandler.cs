using KoobecaFeedController.BL.Request;

namespace KoobecaFeedController.BL.Handlers
{
    public class GreetingHandler : ActivityHandler
    {
        public GreetingHandler(uint userId, RequestParams reqParams) : base(userId, reqParams)
        {
        }

        public override void Execute()
        {
            Response = $"{"greetings":[]}"; //TODO handle greetings
        }
    }
}