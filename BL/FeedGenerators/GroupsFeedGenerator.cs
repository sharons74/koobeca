using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KoobecaFeedController.BL.Request;

namespace KoobecaFeedController.BL.FeedGenerators
{
    public class GroupsFeedGenerator : FeedGenerator
    {
        public GroupsFeedGenerator(RequestParams request) : base(request)
        {
        }

        protected override SourceData[] GetFeedSources()
        {
            return UsersNetworks.Instance.GetUserNetwork(viewerId).GetMembers(memberFilter).Select(m => m.Source).ToArray();
        }

        protected override NetworkMemberFilter CreateMemberFilter()
        {
            memberFilter = base.CreateMemberFilter();
            memberFilter.MemberType = NetworkMemberType.Group;
            return memberFilter;
        }
    }
}
