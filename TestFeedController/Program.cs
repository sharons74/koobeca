using KoobecaFeedController;
using KoobecaFeedController.BL;
using KoobecaFeedController.DAL.Adapters;
using KoobecaFeedController.DAL.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using KoobecaFeedController.BL.Rendering;
using KoobecaFeedController.BL.Handlers;
using System.Text.RegularExpressions;
using KoobecaFeedController.BL.Request;
using KoobecaFeedController.BL.FeedGenerators;
using System.Web;
using KoobecaSync.APIs.FB;

namespace TestFeedController {
    class Program {
        static void Main(string[] args) {

            var user = Users.GetFullById(3);// GetAll().Where(u => u.email == "salmon.sharon@gmail.com").FirstOrDefault();

            MembersBrowseHandler h = new MembersBrowseHandler(0, new RequestParams());
            h.Execute();
            var ums = UserMemberships.Get(3).Where(u => u.active == true && u.user_approved == false).ToList();
            var users = Users.GetAllFull();

            var act = ActivityActions.GetById(11526);
            //var vid = Videos.GetById(2047);
            //GetFeedHandler feedHandler = new GetFeedHandler(3, new RequestParams() { filter_type = "all" });
            //feedHandler.Execute();
            //var json = feedHandler.Response;

            //Logger.Instance.Level = LogLevel.Info;
            //PrintTime("get source", new Action(() =>
            //{
            //    for (int i = 0; i < 100; i++)
            //    {
            //        GetFeedHandler feedHandler2 = new GetFeedHandler(3, new RequestParams() { filter_type = "all" });
            //        feedHandler2.Execute();
            //    }
            //}));
            //PageSources.GetByType("fb_page").ForEach(s => Console.WriteLine($"x{s.source_id}x"));
            //var x = PageSources.GetBySourceId("18793419640", "fb_page");

            //var f = CoreSettings.Get("advancedactivity.feed.font.size", "20");
            //ResponseBody resp = new ResponseBody(random pages as source FeedWrapper[0]);
            //var json= JsonConvert.SerializeObject(resp);
            //var feedHandler = HandlerFactory.CreateHandler("feed", 370, new RequestParams() { subject_id = 370 , subject_type = "user"}, new Dictionary<string, object>());
            //feedHandler.Execute();

            GetActivitiesFromServer();
            Random r = new Random();
            //List<int> simplesort = new List<int>();
            //for (int i = 0; i < 1000; i++)
            //{
            //    simplesort.Add(r.Next(100000));
            //}

            //PrintTime("sort time", new Action(() =>
            //{
            //    for (int i = 0; i < 10000; i++)
            //    {
            //        var copyList = simplesort.ToList();
            //        copyList.Sort();
            //    }
            //}));
            //PrintTime("reverse enumaration", new Action(() =>
            //{
            //    for (int i = 0; i < 100000; i++)
            //    {
            //        foreach (var f in ll.Reverse<int>() ){ }
            //    }
            //}));

            //var src = SourcesMap.Instance.GetSource(3, "user");
            //var sf = src.Feed;
            //foreach(var sssss in sf.Reverse()) { }
            //var U = Users.GetAll().Where(u => u.user_id == 306).ToArray(); 
            ////var z = Storages.GetByFileId(1);
            ////z = Storages.GetByFileId(1);
            //var x = Users.GetAll();
            ////PrintTime("initial db read", new Action(() => DbAccess.Instance.InitialDBRead()));
            //var d = 1;

            ////var rc = ActivityActions.Get(306, 20);



            ////var actions = rc.Select(act => act.action_id).ToArray();
            ////uint actNum = 10000;
            //PrintTime("get from DB", new Action(() =>
            //{
            //    for (int i = 0; i < 500; i++)
            //    {
            //        var r = ActivityActions.GetByID((uint)i,false);
            //    }
            //}));

            //double y = RandomBytes.Rand.Next(200) / 1.2;
            //byte a = (byte)y;
            //int count = 100000000;
            //var x = RandomBytes.Next;
            ////PrintTime($"Create {count} randoms", new Action(()=>CreateRandoms(count)));
            ////PrintTime($"Create {count} regular randoms", new Action(() => CreateRegularRandoms(count)));
            ////PrintTime($"Create {count} randoms", new Action(() => CreateRandoms(count)));

            ////ListToArrayTest();
            //PrintTime("LIst to array test", ListToArrayTest);
            //PrintTime("Array copy test", ArrayCopyTest);

            ulong userId = 3;

            ////set categories for the user
            //SourcesMap.Instance.GetSource(userId);

            //ulong[] sources = null;
            //PrintTime("Generate User Sources", () => { sources = FeedController.Instance.GetUserSources(userId).Select(s => s.MemberId).ToArray(); });
            ////var s = 
            //PrintTime("Creating 1000000 items feed", () => ItemFeed.Instance.Init());

            //PrintTime("Generating Random Activity", () => GenerateRandomActivity(sources, 20));
            //  PrintTime("Getting 1 user's activityfeed", () => FeedController.Instance.GetActivityFeed(userId, 60));

            

            
            
        

            //PrintTime("Getting 1000 mobile user's activityfeed", () => {
            //    //var x22 = File.ReadAllText(@"C:\Workspace\feedtests\10757.txt");
            //    //var ooo = JsonConvert.DeserializeObject(x22);
            //    RequestParams request = new RequestParams()
            //    {
            //        viewerId = (uint)userId,
            //        subject_id = (uint)userId,
            //        subject_type = "user"
            //    };
            //    for (var i = 0; i < 1000; i++) {
            //    FeedGenerator generator = new SourceFeedGenerator(request);
            //    var feed = generator.GetFeedJson();
            //        //var feed = FeedController.Instance.GetActivityFeed(userId, 60000);
            //        //var resultJson = JsonConvert.SerializeObject(feed);
            //    }
            //});
           // PrintTime("Get 1000000 source activites", () => TestGetSourceActivites(sources[0], 1000000));

            PrintTime("DB Get Updated Network members", () =>
             {
                 for (var i = 0; i < 1000000; i++)
                 {
                     DbAccess.Instance.UpdateAllNetworkMembers();
                 }
                 return;
             });


           // PrintTime("Add 1000 activities", () => TestAddActivity(1000));

            PrintTime("DB Get Updated SourceData", () =>
            {
                for (var i = 0; i < 1000; i++)
                {
                    DbAccess.Instance.UpdateSourceData();
                }
                return;
            });

            var sw = new Stopwatch();
            sw.Start();
            //var items = ItemFeed.Instance.QuickList;
            //for (int i = 0; i < 100; i++)
            //{
            //    //var controller = new FeedController();
            //    var sources = FeedController.Instance.GetUserSources(i);
            //}
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
            Console.ReadLine();

        }

        private static void TestIsLike()
        {
            var res = new List<bool>(1000000);
            var likes = new Dictionary<ulong, string>();
            for (ulong i = 0; i < 10000; i++)
            {
                likes[i] = "x";
            }
            for (ulong i = 0; i < 1000000000; i++)
            {
                res.Add(likes.ContainsKey(i));
            }
        }

        private static void GetActivitiesFromServer()
        {
           // var ccc = Comments.GetForComment(861);
           // LikesCommentsHandler lch = new LikesCommentsHandler(3, new RequestParams() { action_id = 37409 });
           // lch.Execute();
        //    LikesHandler ulike = new LikesHandler(3, new RequestParams() { action_id = 37072 });
        //    ulike.Execute();

            var cl = Likes.GetByResourceId(29846);
            //LikesCommentsHandler lch = new LikesCommentsHandler(3, new RequestParams() { action_id = 28869 , viewAllComments = "1" });
            //lch.Execute();
            // var u = Users.GetById(3);
            //var s = Storages.GetByFileId(u.photo_id);
            // Dictionary<string, object> apar = new Dictionary<string, object>();
            // apar["action_id"] = "18021";// "17998";
            // ViewHandler vh = new ViewHandler(3, new RequestParams(), apar);
            // vh.Execute();
            //RemoveActivityHandler rh = new RemoveActivityHandler(3, new RequestParams() { action_id = 20811 });
            //rh.Execute();
          //  PostHandler ph = new PostHandler(339, new RequestParams() { action_id = 31633 });
          // ph.Execute();

            //var x = CoreSettings.Get("core.iframely.secretIframelyKey");//All().Where(s => s.Key.Contains("iframe")).ToArray();
            using (var wc = new WebClient())
            {

                //var photo = AlbumPhotos.GetById(2410);
                //var link = CoreLinks.GetById(12095);
                // var video = Videos.GetById(748);
                //var refs = ActivityActions.GetByObjectId(8005, "activity_action").ToArray();
                //SourceData han = SourcesMap.Instance.GetSource(368, "user");
                // var hanFeed = han.ActivityFeed.Feed;



                uint aid = 29846;// 26864;// 26843;
                var activity = ActivityActions.GetById(aid);
                var a = new KoobecaFeedController.BL.Activity(activity);
                var wrapper = new FeedWrapper(a,3,"user",false);
                var res = JsonConvert.SerializeObject(wrapper,Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });
                var ourFeeds = new List<FeedWrapper>();

                //FeedController.Instance.AddLike(3, activity.action_id);
                //FeedController.Instance.RemoveLike(3, activity.action_id);
                var sb = new StringBuilder();
                //foreach (var feed in json.body.data)
                //{
                //    uint action_id = feed.feed.action_id;
                //    sb.AppendLine(action_id.ToString());
                //    var rawActivity = ActivityActions.GetByID(action_id);
                //    if (action_id == aid) {
                //        if (rawActivity != null)
                //        {
                //            sb.AppendLine(action_id.ToString());
                //            ourFeeds.Add(new FeedWrapper(new KoobecaFeedController.BL.Activity(rawActivity)));
                //        }
                //    }
                //}
                var str = sb.ToString();
                var result = JsonConvert.SerializeObject(new ResponseBody(ourFeeds.ToArray(),0,0,50), Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });

            }
        }

        //private static void TestAddActivity(int count)
        //{
        //    for (int i = 0; i < count; i++)
        //    {
        //        FeedController.Instance.AddActivity(new KoobecaFeedController.BL.Activity
        //        {
        //            ActivityId = (ulong)RandomBytes.Rand.Next(1000000),
        //            InnerActivityId = (ulong)RandomBytes.Rand.Next(1000000),
        //            ActivityTime = LocalTime.CurrentIntSecond,
        //            Type = ActivityType.Like
        //        });
        //    }
        //}

        private static void CreateRandoms(int count) {
            var x = RandomBytes.Next;
            for (var i = 0; i < count; i++) {
                x = RandomBytes.Next;
            }
        }

        private static void CreateRegularRandoms(int count) {
            var r = RandomBytes.Rand;
            var x = r.Next(63);
            for (var i = 0; i < count; i++) {
                x = r.Next(63);
            }
        }

        

        private static void ListToArrayTest() {
            var result = new List<object>();
            var list = new List<long>();
            for (var i = 0; i < 1000; i++) {
                list.Add(i);
            }
            for (var i = 0; i < 10000; i++) {
                list.Add(i);
                result.Add(list.ToArray());
            }
        }

        private static void ArrayCopyTest() {
            var baseArray = new long[1000];


            for (var i = 0; i < 10000; i++) {
                var newArray = new long[900];
                Array.Copy(baseArray, newArray, newArray.Length);
            }
        }

        private static void PrintTime(string msg, Action p) {
            var sw = new Stopwatch();
            sw.Start();
            p.Invoke();
            sw.Stop();
            Console.WriteLine($"{msg} took {sw.ElapsedMilliseconds} ms");
        }

        private static void GenerateRandomActivity(ulong[] sources, int countPerSource = 10) {
            foreach (var source in sources) {
                var feed = SourcesMap.Instance.GetSource(source,"user").ActivityFeed; // that will couse generation through DAL
            }
        }
    }


}
