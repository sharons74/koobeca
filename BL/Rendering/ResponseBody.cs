using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace KoobecaFeedController.BL.Rendering {
    public class ResponseBody {
        public FeedWrapper[] data = new FeedWrapper[0];
        public string defaultFeedCount = "50";
        public uint activityCount = 50;
        public uint accurateActivityCount = 50;
        public bool enable_composer = true;
        public bool enable_composer_photo = true;
        public uint maxid;
        public uint minid;
        public FilterTab[] filterTabs;
        public object video_source;
        public object feed_post_menu;
        public uint reactionsEnabled = 1;
        public uint stickersEnabled = 1;
        public uint emojiEnabled = 1;
        public string showFilterType = "1";
        public string is_show_greeting_announcement = "1";
        public object feedDecorationSetting;
        public object reactions;
        public object statusBoxSetting;
        public uint notifyItemAt;

        public ResponseBody() { } //for json deserialies option

        public ResponseBody(FeedWrapper[] feed, ulong minId, ulong maxId, int feedSizeLimit) {
            activityCount = (uint)feedSizeLimit;
            accurateActivityCount = (uint)feed.Length;
            data = feed;
            filterTabs = GetFilterTabs();
            maxid = (uint)maxId;
            minid = (uint)minId;
            feed_post_menu = GetFeedPostMenu();
            feedDecorationSetting = GetDecorationSettings();
            video_source = GetVideoSource();
            accurateActivityCount = (uint)feed.Length;
            //statusBoxSetting = GetStatusBoxSetting();
            reactions = GetReactions();
        }


        private object GetStatusBoxSetting() {
            return new {
                allowBanner = 1,
                allowMemories = 1,
                allowGreeting = 1,
                allowPin = 1,
                allowfeelingActivity = 1,
                allowSchedulePost = 1,
                allowTargetPost = 1
            };
        }

        private object GetVideoSource() {
            //return new[] {
            //    "YouTube",
            //    "Vimeo",
            //    "My Device"
            //};
            Dictionary<string, string> source = new Dictionary<string, string>();
            source["1"] = "Youtube";
            source["2"] = "Vimeo";
            source["3"] = "My Device";
            return source;
        }

        private object GetDecorationSettings() {
            return new {
                char_length = "100",
                font_size = "30",
                font_color = "#000",
                font_style = "normal",
                banner_feed_length = "100",
                banner_count = "10",
                banner_order = "random"
            };
        }

        private object GetFeedPostMenu() {
            return new {
                status = 1,
                withtags = 1,
                emotions = 1,
                checkin = 0,
                photo = 1,
                video = 1,
                music = 0,
                link = 1,
                userprivacy = new Dictionary<string, string> {
                    {"everyone", "Everyone"},
                    {"friends", "Friends"},
                    {"onlyme", "Me"}
                },
                allowAdvertize = 0,
                allowBanner = 1,
                allowMemories = 1,
                allowGreeting = 1,
                allowPin = 1,
                allowfeelingActivity = 0,
                allowSchedulePost = 0,
                allowTargetPost = 1
            };
        }

        private FilterTab[] GetFilterTabs() {

            var allAupates = new FilterTab("All", "all", 1);
            var friends = new FilterTab("Friends", "membership", 2);
            var pages = new FilterTab("Pages", "sitepage", 8);
            var groups = new FilterTab("Groups", "sitegroup", 22);
            var saved = new FilterTab("Saved", "user_saved", 21);
            var events = new FilterTab("Events", "siteevent", 26);
            var hidden = new FilterTab("Hidden", "hidden_post", 28);

            //hidden_post

            return new FilterTab[] { allAupates, friends,groups,saved };
        }

        private static object GetReactions() {
            return new {
                like = new {
                    caption = "Like",
                    order = 1,
                    reactionicon_id = 1,
                    reaction = "like",
                    icon = new {
                        reaction_image_icon =
                            "https://d18js22wsvyojh.cloudfront.net/public/system/0b/6c89b7e22388a5328752219c5501257f.png"
                    }
                },
                love = new {
                    caption = "Love",
                    order = 2,
                    reactionicon_id = 2,
                    reaction = "love",
                    icon = new {
                        reaction_image_icon =
                            "https://d18js22wsvyojh.cloudfront.net/public/system/0d/c14e97adff7381b08007b69d86640af7.png"
                    }
                },
                wow = new {
                    caption = "WOW",
                    order = 3,
                    reactionicon_id = 3,
                    reaction = "wow",
                    icon = new {
                        reaction_image_icon =
                            "https://d18js22wsvyojh.cloudfront.net/public/system/0f/5cebb9ab3ded62211c7f37dc82269f51.png"
                    }
                },
                haha = new {
                    caption = "HaHa",
                    order = 4,
                    reactionicon_id = 4,
                    reaction = "haha",
                    icon = new {
                        reaction_image_icon =
                            "https://d18js22wsvyojh.cloudfront.net/public/system/11/5a26fa214a08a14dac8cd224f7490676.png"
                    }
                },
                sad = new {
                    caption = "Sad",
                    order = 5,
                    reactionicon_id = 5,
                    reaction = "sad",
                    icon = new {
                        reaction_image_icon =
                            "https://d18js22wsvyojh.cloudfront.net/public/system/13/3eec208927e914d9b1d9f4f4066d1d48.png"
                    }
                },
                angry = new {
                    caption = "Angry",
                    order = 6,
                    reactionicon_id = 6,
                    reaction = "angry",
                    icon = new {
                        reaction_image_icon =
                            "https://d18js22wsvyojh.cloudfront.net/public/system/15/88dde8bbafcccc542c70f2a161878f92.png"
                    }
                }
            };
        }

        public static ResponseBody CreateForIOS()
        {

            string sb = "{" +
    "  \"defaultFeedCount\": \"50\"," +
    "  \"activityCount\": 50," +
    "  \"accurateActivityCount\": 50," +
    "  \"enable_composer\": true," +
    "  \"enable_composer_photo\": true," +
    "  \"maxid\": 10741," +
    "  \"minid\": 11620," +
    "  \"filterTabs\": [" +
    "    {" +
    "      \"tab_title\": \"All Updates\"," +
    "      \"urlParams\": {" +
    "        \"filter_type\": \"all\"," +
    "        \"list_id\": 1," +
    "        \"isFromTab\": 1," +
    "        \"feedOnly\": 1" +
    "      }" +
    "    }," +
    "    {" +
    "      \"tab_title\": \"Friends\"," +
    "      \"urlParams\": {" +
    "        \"filter_type\": \"membership\"," +
    "        \"list_id\": 2," +
    "        \"isFromTab\": 1," +
    "        \"feedOnly\": 1" +
    "      }" +
    "    }," +
    "    {" +
    "      \"tab_title\": \"Photos\"," +
    "      \"urlParams\": {" +
    "        \"filter_type\": \"photo\"," +
    "        \"list_id\": 3," +
    "        \"isFromTab\": 1," +
    "        \"feedOnly\": 1" +
    "      }" +
    "    }," +
    "    {" +
    "      \"tab_title\": \"Posts\"," +
    "      \"urlParams\": {" +
    "        \"filter_type\": \"posts\"," +
    "        \"list_id\": 4," +
    "        \"isFromTab\": 1," +
    "        \"feedOnly\": 1" +
    "      }" +
    "    }," +
    "    {" +
    "      \"tab_title\": \"Music\"," +
    "      \"urlParams\": {" +
    "        \"filter_type\": \"music\"," +
    "        \"list_id\": 6," +
    "        \"isFromTab\": 1," +
    "        \"feedOnly\": 1" +
    "      }" +
    "    }," +
    "    {" +
    "      \"tab_title\": \"Videos\"," +
    "      \"urlParams\": {" +
    "        \"filter_type\": \"video\"," +
    "        \"list_id\": 7," +
    "        \"isFromTab\": 1," +
    "        \"feedOnly\": 1" +
    "      }" +
    "    }," +
    "    {" +
    "      \"tab_title\": \"Pages\"," +
    "      \"urlParams\": {" +
    "        \"filter_type\": \"sitepage\"," +
    "        \"list_id\": 8," +
    "        \"isFromTab\": 1," +
    "        \"feedOnly\": 1" +
    "      }" +
    "    }," +
    "    {" +
    "      \"tab_title\": \"Scheduled Posts\"," +
    "      \"urlParams\": {" +
    "        \"filter_type\": \"schedule_post\"," +
    "        \"list_id\": 27," +
    "        \"isFromTab\": 1," +
    "        \"feedOnly\": 1" +
    "      }" +
    "    }," +
    "    {" +
    "      \"tab_title\": \"Hidden Posts\"," +
    "      \"urlParams\": {" +
    "        \"filter_type\": \"hidden_post\"," +
    "        \"list_id\": 28," +
    "        \"isFromTab\": 1," +
    "        \"feedOnly\": 1" +
    "      }" +
    "    }," +
    "    {" +
    "      \"tab_title\": \"On This Day\"," +
    "      \"urlParams\": {" +
    "        \"filter_type\": \"memories\"," +
    "        \"list_id\": 29," +
    "        \"isFromTab\": 1," +
    "        \"feedOnly\": 1" +
    "      }" +
    "    }," +
    "    {" +
    "      \"tab_title\": \"Buy Sell\"," +
    "      \"urlParams\": {" +
    "        \"filter_type\": \"advertise\"," +
    "        \"list_id\": 30," +
    "        \"isFromTab\": 1," +
    "        \"feedOnly\": 1" +
    "      }" +
    "    }," +
    "    {" +
    "      \"tab_title\": \"Event\"," +
    "      \"urlParams\": {" +
    "        \"filter_type\": \"siteevent\"," +
    "        \"list_id\": 26," +
    "        \"isFromTab\": 1," +
    "        \"feedOnly\": 1" +
    "      }" +
    "    }," +
    "    {" +
    "      \"tab_title\": \"Saved Feeds\"," +
    "      \"urlParams\": {" +
    "        \"filter_type\": \"user_saved\"," +
    "        \"list_id\": 21," +
    "        \"isFromTab\": 1," +
    "        \"feedOnly\": 1" +
    "      }" +
    "    }," +
    "    {" +
    "      \"tab_title\": \"Groups\"," +
    "      \"urlParams\": {" +
    "        \"filter_type\": \"sitegroup\"," +
    "        \"list_id\": 22," +
    "        \"isFromTab\": 1," +
    "        \"feedOnly\": 1" +
    "      }" +
    "    }" +
    "  ]," +
    "  \"video_source\": {" +
    "    \"1\": \"YouTube\"," +
    "    \"2\": \"Vimeo\"," +
    "    \"3\": \"My Device\"," +
    "    \"4\": \"Dailymotion\"" +
    "  }," +
    "  \"feed_post_menu\": {" +
    "    \"status\": 1," +
    "    \"withtags\": 1," +
    "    \"emotions\": 1," +
    "    \"photo\": 1," +
    "    \"checkin\": 1," +
    "    \"video\": 1," +
    "    \"music\": 0," +
    "    \"link\": 1," +
    "    \"userprivacy\": {" +
    "      \"everyone\": \"Everyone\"," +
    "      \"networks\": \"Friends & Networks\"," +
    "      \"friends\": \"Friends Only\"," +
    "      \"onlyme\": \"Only Me\"," +
    //"      \"network_17\": \"Art & Culture\"," +
    //"      \"network_21\": \"Bulletin board\"," +
    //"      \"network_14\": \"Business & Finance\"," +
    //"      \"network_13\": \"From the world\"," +
    //"      \"network_18\": \"Health & Lifestyle\"," +
    //"      \"network_12\": \"News\"," +
    //"      \"network_24\": \"Other topics\"," +
   // "      \"network_22\": \"Personal expression\"," +
   // "      \"network_16\": \"Politics & Opinions\"," +
    //"      \"network_23\": \"Shopping and sales\"," +
   // "      \"network_15\": \"Sport\"," +
   // "      \"network_19\": \"Tech & Science\"," +
   // "      \"network_20\": \"Travel & Leisure\"," +
   // "      \"network_list_custom\": \"Multiple Networks\"," +
   // "      \"friend_list_custom\": \"Multiple Friend Lists\"" +
    "    }," +
    //"    \"multiple_networklist\": [" +
    //"      {" +
    //"        \"type\": \"Checkbox\"," +
    //"        \"name\": \"network_17\"," +
    //"        \"label\": \"Art & Culture\"," +
    //"        \"value\": 0" +
    //"      }," +
    //"      {" +
    //"        \"type\": \"Checkbox\"," +
    //"        \"name\": \"network_21\"," +
    //"        \"label\": \"Bulletin board\"," +
    //"        \"value\": 0" +
    //"      }," +
    //"      {" +
    //"        \"type\": \"Checkbox\"," +
    //"        \"name\": \"network_14\"," +
    //"        \"label\": \"Business & Finance\"," +
    //"        \"value\": 0" +
    //"      }," +
    //"      {" +
    //"        \"type\": \"Checkbox\"," +
    //"        \"name\": \"network_13\"," +
    //"        \"label\": \"From the world\"," +
    //"        \"value\": 0" +
    //"      }," +
    //"      {" +
    //"        \"type\": \"Checkbox\"," +
    //"        \"name\": \"network_18\"," +
    //"        \"label\": \"Health & Lifestyle\"," +
    //"        \"value\": 0" +
    //"      }," +
    //"      {" +
    //"        \"type\": \"Checkbox\"," +
    //"        \"name\": \"network_12\"," +
    //"        \"label\": \"News\"," +
    //"        \"value\": 0" +
    //"      }," +
    //"      {" +
    //"        \"type\": \"Checkbox\"," +
    //"        \"name\": \"network_24\"," +
    //"        \"label\": \"Other topics\"," +
    //"        \"value\": 0" +
    //"      }," +
    //"      {" +
    //"        \"type\": \"Checkbox\"," +
    //"        \"name\": \"network_22\"," +
    //"        \"label\": \"Personal expression\"," +
    //"        \"value\": 0" +
    //"      }," +
    //"      {" +
    //"        \"type\": \"Checkbox\"," +
    //"        \"name\": \"network_16\"," +
    //"        \"label\": \"Politics & Opinions\"," +
    //"        \"value\": 0" +
    //"      }," +
    //"      {" +
    //"        \"type\": \"Checkbox\"," +
    //"        \"name\": \"network_23\"," +
    //"        \"label\": \"Shopping and sales\"," +
    //"        \"value\": 0" +
    //"      }," +
    //"      {" +
    //"        \"type\": \"Checkbox\"," +
    //"        \"name\": \"network_15\"," +
    //"        \"label\": \"Sport\"," +
    //"        \"value\": 0" +
    //"      }," +
    //"      {" +
    //"        \"type\": \"Checkbox\"," +
    //"        \"name\": \"network_19\"," +
    //"        \"label\": \"Tech & Science\"," +
    //"        \"value\": 0" +
    //"      }," +
    //"      {" +
    //"        \"type\": \"Checkbox\"," +
    //"        \"name\": \"network_20\"," +
    //"        \"label\": \"Travel & Leisure\"," +
    //"        \"value\": 0" +
    //"      }" +
    //"    ]," +
    "    \"allowTargetPost\": 1," +
    "    \"allowSchedulePost\": 1," +
    "    \"allowfeelingActivity\": 1," +
    "    \"allowAdvertize\": 0," +
    "    \"allowPin\": 1," +
    "    \"allowGreeting\": 1," +
    "    \"allowMemories\": 1," +
    "    \"allowBanner\": 1" +
    "  }," +
    "  \"reactionsEnabled\": 1," +
    "  \"stickersEnabled\": 1," +
    "  \"emojiEnabled\": 1," +
    "  \"showFilterType\": \"1\"," +
    "  \"is_show_greeting_announcement\": \"1\"," +
    "  \"feedDecorationSetting\": {" +
    "    \"char_length\": \"100\"," +
    "    \"font_size\": \"15\"," +
    "    \"font_color\": \"#000\"," +
    "    \"font_style\": \"normal\"," +
    "    \"banner_feed_length\": \"100\"," +
    "    \"banner_count\": \"10\"," +
    "    \"banner_order\": \"random\"" +
    "  }," +
    "  \"reactions\": {" +
    "    \"like\": {" +
    "      \"caption\": \"Like\"," +
    "      \"order\": 1," +
    "      \"reactionicon_id\": 1," +
    "      \"reaction\": \"like\"," +
    "      \"icon\": {" +
    "        \"reaction_image_icon\": \"https://d18js22wsvyojh.cloudfront.net/public/system/0b/6c89b7e22388a5328752219c5501257f.png\"" +
    "      }" +
    "    }," +
    "    \"love\": {" +
    "      \"caption\": \"Love\"," +
    "      \"order\": 2," +
    "      \"reactionicon_id\": 2," +
    "      \"reaction\": \"love\"," +
    "      \"icon\": {" +
    "        \"reaction_image_icon\": \"https://d18js22wsvyojh.cloudfront.net/public/system/0d/c14e97adff7381b08007b69d86640af7.png\"" +
    "      }" +
    "    }," +
    "    \"wow\": {" +
    "      \"caption\": \"WOW\"," +
    "      \"order\": 3," +
    "      \"reactionicon_id\": 3," +
    "      \"reaction\": \"wow\"," +
    "      \"icon\": {" +
    "        \"reaction_image_icon\": \"https://d18js22wsvyojh.cloudfront.net/public/system/0f/5cebb9ab3ded62211c7f37dc82269f51.png\"" +
    "      }" +
    "    }," +
    "    \"haha\": {" +
    "      \"caption\": \"HaHa\"," +
    "      \"order\": 4," +
    "      \"reactionicon_id\": 4," +
    "      \"reaction\": \"haha\"," +
    "      \"icon\": {" +
    "        \"reaction_image_icon\": \"https://d18js22wsvyojh.cloudfront.net/public/system/11/5a26fa214a08a14dac8cd224f7490676.png\"" +
    "      }" +
    "    }," +
    "    \"sad\": {" +
    "      \"caption\": \"Sad\"," +
    "      \"order\": 5," +
    "      \"reactionicon_id\": 5," +
    "      \"reaction\": \"sad\"," +
    "      \"icon\": {" +
    "        \"reaction_image_icon\": \"https://d18js22wsvyojh.cloudfront.net/public/system/13/3eec208927e914d9b1d9f4f4066d1d48.png\"" +
    "      }" +
    "    }," +
    "    \"angry\": {" +
    "      \"caption\": \"Angry\"," +
    "      \"order\": 6," +
    "      \"reactionicon_id\": 6," +
    "      \"reaction\": \"angry\"," +
    "      \"icon\": {" +
    "        \"reaction_image_icon\": \"https://d18js22wsvyojh.cloudfront.net/public/system/15/88dde8bbafcccc542c70f2a161878f92.png\"" +
    "      }" +
    "    }," +
    "    \"fu\": {" +
    "      \"caption\": \"FU\"," +
    "      \"order\": 7," +
    "      \"reactionicon_id\": 7," +
    "      \"reaction\": \"fu\"," +
    "      \"icon\": {" +
    "        \"reaction_image_icon\": \"https://d18js22wsvyojh.cloudfront.net/public/system/05/07/d96f377450509f439ce05ff690188833.png\"" +
    "      }" +
    "    }" +
    "  }," +
    "  \"notifyItemAt\": 0" +
    "}";

            return JsonConvert.DeserializeObject<ResponseBody>(sb.ToString());
        }
    }

    public class FilterTab
    {

        public FilterTab(string title,string type,int id)
        {
            tab_title = title;
            urlParams = new FilterTabUrlParams() { filter_type = type, list_id = id };
        }

        public string tab_title;
        public FilterTabUrlParams  urlParams ;

    }

    public class FilterTabUrlParams
    {
        public string filter_type;
        public int list_id;
        public int isFromTab = 1;
        public int feedOnly = 1;
    }
}