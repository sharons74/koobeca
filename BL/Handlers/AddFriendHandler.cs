using KoobecaFeedController.BL.Request;
using KoobecaFeedController.DAL.Adapters;

namespace KoobecaFeedController.BL.Handlers
{
    internal class AddFriendHandler : ActivityHandler
    {
        public AddFriendHandler(uint userId, RequestParams reqParams) : base(userId, reqParams)
        { 
           
        }

        public override void Execute()
        {
            UserMemberships.AddUserFreindRequest(userId, reqParams.user_id);
        }
    
    }
}