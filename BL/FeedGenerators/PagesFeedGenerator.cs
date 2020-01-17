using KoobecaFeedController.BL.Request;
using System.Linq;

namespace KoobecaFeedController.BL.FeedGenerators
{
    public class PagesFeedGenerator : FeedGenerator
    {
        public PagesFeedGenerator(RequestParams request) : base(request)
        {
        }

        protected override SourceData[] GetFeedSources()
        {
            return UsersNetworks.Instance.GetFollowedSources(viewerId).List.Where(m=>m.MemberType == memberFilter.MemberType).Select(m => m.Source).ToArray();
        }

        protected override NetworkMemberFilter CreateMemberFilter()
        {
            memberFilter = base.CreateMemberFilter();
            memberFilter.MemberType = NetworkMemberType.Page;
            return memberFilter;
        }
    }

}
