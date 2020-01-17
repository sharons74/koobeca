using KoobecaFeedController.BL.Request;
using KoobecaFeedController.DAL.Adapters;

namespace KoobecaFeedController.BL.Handlers
{
    internal class FriendRemoveHandler : ActivityHandler
    {
        public FriendRemoveHandler(uint userId, RequestParams reqParams) : base(userId, reqParams)
        {
        }

        public override void Execute()
        {
            UserMemberships.RemoveFriendRequest(userId, reqParams.user_id);
        }
    }
}