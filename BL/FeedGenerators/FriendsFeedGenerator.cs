using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KoobecaFeedController.BL.Request;

namespace KoobecaFeedController.BL.FeedGenerators
{
    public class FriendsFeedGenerator : FeedGenerator
    {
        public FriendsFeedGenerator(RequestParams request) : base(request)
        {
        }

        protected override SourceData[] GetFeedSources()
        {
            var friendsList = UsersNetworks.Instance.GetUserNetwork(viewerId).GetMembers(memberFilter).Select(m=>m.Source).ToList();
            //friendsList.Add(viewer);
            return friendsList.ToArray();
        }

        protected override NetworkMemberFilter CreateMemberFilter()
        {
            memberFilter = base.CreateMemberFilter();
            memberFilter.MemberType = NetworkMemberType.User;
            return memberFilter;
        }


    }
}
