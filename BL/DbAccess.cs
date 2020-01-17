using System.Collections.Generic;
using System.Linq;
using KoobecaFeedController.DAL.Adapters;

namespace KoobecaFeedController.BL {
    public class DbAccess {
        private DbAccess() { }
        public static DbAccess Instance { get; } = new DbAccess();

        public SourceData GetSourceData(ulong sourceId, string sourceType) {
            var newSource = new SourceData {SourceId = sourceId, SourceType = sourceType};
            newSource.SetCategories(CreateAllCategories());
            newSource.Languages = 0x07; //0111
            newSource.FetchDate = LocalTime.CurrentIntSecond;
            return newSource;
        }

        public List<NetworkMember> GetNetworkMembers(ulong sourceId) {
            var members = UserMemberships.Get((uint) sourceId).Where(m => m.active && m.resource_id != 339)
                .Select(m => new NetworkMember {AffinityLevel = 50, MemberId = m.resource_id}).ToList(); 
            var groups = Groups.GetUserGroups((int)sourceId).Select(g => new NetworkMember() { MemberId = (ulong)g, MemberType = NetworkMemberType.Group }).ToList();
            members.AddRange(groups);
            return members;
            //return GetRandomNetworkMembers();
        }

        public void UpdateAllNetworkMembers() {
            //need to get all the network members that where updated since last fetch
            var updatedList = UsersNetworks.Instance.GetUpdatedNetworkMembers();
            // and push them to DB
            // throw new NotImplementedException();
        }

        public void UpdateSourceData() {
            //need to get all sources that had change in affinity
            var updated = SourcesMap.Instance.GetUpdatedCategories();
            // and push them to DB
            // throw new NotImplementedException();
        }


        #region Simulation methods

        private List<NetworkMember> GetRandomNetworkMembers() {
            var members = new List<NetworkMember>();
            for (var i = 0; i < 1000; i++)
                members.Add(new NetworkMember {
                    AffinityLevel = (byte) RandomBytes.Rand.Next(100),
                    MemberId = (ulong) RandomBytes.Rand.Next(1000000) // randomly one of 1000000 members
                });
            return members;
        }


        public List<CategoryInfo> CreateAllCategories() {
            var res = new List<CategoryInfo>();
            for (var i = 0; i < 20; i++)
            for (var j = 0; j < 20; j++)
                res.Add(new CategoryInfo {AffinityLevel = 100, Category = (byte) i, Subcategory = (byte) j});

            return res;
        }

        /// <summary>
        ///     For testing purposes
        /// </summary>
        public List<CategoryInfo> CreateRandomCategories(byte categoryCount, byte subcategoryPerCategoryCount) {
            var res = new List<CategoryInfo>();

            //create random category list (16 categories)
            var categories = (ulong) 0;
            var categoryList = new List<byte>();
            for (byte i = 0; i < categoryCount; i++) {
                var categoryBit = (byte) RandomBytes.Rand.Next(15); //0-16
                if (!categoryList.Contains(categoryBit))
                    categoryList.Add(categoryBit);
                categories |= (ulong) 1 << categoryBit;
            }

            foreach (var category in categoryList) {
                var subcategories = new List<byte>();

                for (var i = 0; i < subcategoryPerCategoryCount; i++) {
                    var subcategory = (byte) RandomBytes.Rand.Next(9);
                    if (!subcategories.Contains(subcategory)) {
                        subcategories.Add(subcategory);
                        var affinity = RandomBytes.Rand.Next(80) + 20; // 20-100
                        var ci = new CategoryInfo {
                            AffinityLevel = (byte) affinity,
                            Category = category,
                            Subcategory = subcategory
                        };
                        res.Add(ci);
                    }
                }
            }

            return res;
        }

        #endregion
    }
}