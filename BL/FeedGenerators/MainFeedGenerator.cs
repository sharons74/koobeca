using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KoobecaFeedController.BL.Request;
using KoobecaFeedController.DAL.Adapters;

namespace KoobecaFeedController.BL.FeedGenerators
{
    public class MainFeedGenerator : FeedGenerator
    {
        private Dictionary<string, SourceData> _sources = new Dictionary<string, SourceData>();
        private readonly string feedType = "main";

        public MainFeedGenerator(RequestParams request) : base(request)
        {
        }
        
        protected override SourceData[] GetFeedSources()
        {
            string cacheKey = $"{viewerId}|{feedType}";

            List<SourceData> res = FeedSourcesCache.GetCache(cacheKey);

            if (res == null) //not in cache
            {

                //add followed pages
                memberFilter.MemberType = NetworkMemberType.Page;
                UsersNetworks.Instance.GetFollowedSources(viewerId).List.Where(m => m.MemberType == memberFilter.MemberType).
                    Select(m => AddSource(m.Source)).ToList();
                //add liked pages & groups
                UsersNetworks.Instance.GetLikedSources(viewerId).List.Where(s => s.AffinityLevel > 20).Select(m => AddSource(m.Source)).ToList();

                //add membership groups
                memberFilter.MemberType = NetworkMemberType.Group;
                UsersNetworks.Instance.GetUserNetwork(viewerId).GetMembers(memberFilter).Select(m => AddSource(m.Source)).ToList();
                //add friends
                memberFilter.MemberType = NetworkMemberType.User;
                UsersNetworks.Instance.GetUserNetwork(viewerId).GetMembers(memberFilter).Select(m => AddSource(m.Source)).ToList();
                //add viewer
                AddSource(viewer);

                //add public figures
               // AddPublicFigures();

                res = _sources.Values.ToList();
                FeedSourcesCache.SetCache(cacheKey,res);
            }

            //additional sources
            AddPageSourcesIfNeeded(res);
            return res.ToArray();
        }

        private void AddPublicFigures()
        {
            PageSources.GetByType("fb_user").Select(s => AddSource(SourcesMap.Instance.GetSource(s.page_id, "user"))).ToArray();
        }

        private void AddPageSourcesIfNeeded(List<SourceData> list = null)
        {
            var sourceCount = _sources.Count();
            int randomPageCount = sourceCount / 10; //10 % of sources if number of sources is more than 10
            if (sourceCount < 10)
            {
                //add pages as source to match 10
                randomPageCount = 10 - sourceCount;
            }
            AddRandomPages(randomPageCount,list);
        }

        private void AddRandomPages(int randomPageCount,List<SourceData> list = null)
        {
            Logger.Instance.Debug($"Adding {randomPageCount} random pages as source");
            uint maxPageNum = Pages.MaxPageNum;
            for (int i = 0; i < randomPageCount;)
            {
                var pageId = RandomBytes.Rand.Next((int)maxPageNum);
                var pageSource = SourcesMap.Instance.GetSource((ulong)pageId, "sitepage_page");
                if (AddSource(pageSource,list)) {
                    i++;
                    //Logger.Instance.Debug($"added page {pageId} randomally as source");
                }
            }
        }

        private bool AddSource(SourceData source, List<SourceData> list = null)
        {
            if (source == null) return false;
            string key = $"{source.SourceType}:{source.SourceId}";
            if (_sources.ContainsKey(key)) return false;
            _sources[key] = source;
            if (list != null) list.Add(source); //if list is given
            return true;
        }

        protected override NetworkMemberFilter CreateMemberFilter()
        {
            memberFilter = base.CreateMemberFilter();
            memberFilter.MemberType = NetworkMemberType.Page;
            return memberFilter;
        }
    }
}
