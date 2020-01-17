using System;
using System.Collections.Generic;
using System.Linq;
using KoobecaFeedController;
using KoobecaFeedController.BL;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        //[TestMethod]
        //public void TestItemFeed()
        //{
        //    ItemFeed.Instance.Init();
        //    var source = SourcesMap.Instance.GetSource(1,"user");
        //    source.ClearAllCategories();
        //    var categories = new List<CategoryInfo>();
        //    categories.Add(new CategoryInfo() { AffinityLevel = 30, Category = 2, Subcategory = 2 });
        //    categories.Add(new CategoryInfo() { AffinityLevel = 50, Category = 3, Subcategory = 2 });

        //    source.SetCategories(categories);
        //    var items = ItemFeed.Instance.GetItems(1,"user",10,10,false);

        //    Assert.IsTrue(items.Count() > 0);
        //}




        [TestMethod]
        public void TestActivityFeed()
        {
            var source = SourcesMap.Instance.GetSource(1,"user");
           // source.ClearAllCategories();
           // var categories = new List<CategoryInfo>();
           // categories.Add(new CategoryInfo() { AffinityLevel = 30, Category = 2, Subcategory = 2 });
           //// categories.Add(new CategoryInfo() { AffinityLevel = 50, Category = 3, Subcategory = 2 });

           // source.SetCategories(categories);
            //var feed = FeedController.Instance.GetActivityFeed(1, 60);
            //Assert.IsTrue(feed.Length > 0);
        }


        [TestMethod]
        public void TestAffinityChange()
        {
            //ItemFeed.Instance.Init();
            //var source = SourcesMap.Instance.GetSource(1);
            //source.ClearAllCategories();
            //var categories = new List<CategoryInfo>();
            //categories.Add(new CategoryInfo() { AffinityLevel = 30, Category = 2, Subcategory = 2 });
            //// categories.Add(new CategoryInfo() { AffinityLevel = 50, Category = 3, Subcategory = 2 });

            //source.SetCategories(categories);
            //var feed = FeedController.Instance.GetActivityFeed(1, 60);

            ////take the member of the first activity
            //var activity = feed.ElementAt(0).Activities.First().Value;
            //var member = activity.OwnerId;//feed.ElementAt(0).FirstReactor;
            //var affinity = UsersNetworks.Instance.GetUserNetwork(1).GetMember(member).AffinityLevel;
            //FeedController.Instance.AddRelationShip(member, 1,affinity);
            //var msg = GetMutualAffinity(1, member);
            //ActivateMambers(1);
            ////add activity
            //var newActivity = activity.Clone();
            //newActivity.ActivityTime = LocalTime.CurrentIntSecond;
            //newActivity.OwnerId = 1;
            //newActivity.ActivityId = (ulong)RandomBytes.Rand.Next(1000000);
            //newActivity.Type = ActivityType.Like;
            //FeedController.Instance.AddActivity(newActivity);
            //msg = GetMutualAffinity(1, member);

            //DbAccess.Instance.UpdateAllNetworkMembers();
            //DbAccess.Instance.UpdateSourceData();
        }



        private string GetMutualAffinity(ulong member1,ulong member2)
        {
            var m2 =  UsersNetworks.Instance.GetUserNetwork(member1).GetMember(member2);
            var m1 = UsersNetworks.Instance.GetUserNetwork(member2).GetMember(member1);

            return $"{member1} has affinity of {m2?.AffinityLevel} to {member2} , {member2} has affinity of {m1?.AffinityLevel} to {member1} ";
        }

        private void ActivateMambers(ulong source)
        {
            foreach(var member in UsersNetworks.Instance.GetAllMembers(source))
            {
                SourcesMap.Instance.GetSource(member.MemberId,member.Type).LastActivity = LocalTime.CurrentIntSecond - 25 * 3600;
            }

        }
    }
}
