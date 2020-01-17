using KoobecaFeedController.BL.Request;

namespace KoobecaFeedController.BL.Handlers
{
    internal class StoryCreateHandler : ActivityHandler
    {
        public StoryCreateHandler(uint userId, RequestParams reqParams) : base(userId, reqParams)
        {
        }

        public override void Execute()
        {
            base.Execute();
        }
    }
}