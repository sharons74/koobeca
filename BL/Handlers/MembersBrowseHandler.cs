using KoobecaFeedController.BL.Rendering;
using KoobecaFeedController.BL.Request;
using KoobecaFeedController.DAL.Adapters;
using KoobecaFeedController.DAL.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KoobecaFeedController.BL.Handlers
{
    public class MembersBrowseHandler : ActivityHandler
    {
        private string _Filter;

        public MembersBrowseHandler(uint userId, RequestParams reqParams) : base(userId, reqParams)
        {
            _Filter = reqParams.DisplayName ?? "";
            _Filter = _Filter.ToLower();
        }

        public override void Execute()
        {

            var members = Users.GetAllFull().Where(m=>m.displayname.ToLower().Contains(_Filter)).ToList();
            MembersResponse res = new MembersResponse(members);

            Response = JsonConvert.SerializeObject(res);
        }

        public class MembersResponse
        {
            private List<UserFull> members;

            public MembersResponse(List<UserFull> members)
            {
                response = members.Select(m => new Member(m)).ToArray();
                totalItemCount = response.Length;
            }

            public int isSitemember { get; set; }
            public int page { get; set; } = 1;
            public Member[] response { get; set; }
            public int totalItemCount { get; set; }
        }

        public class Member
        {
            public int user_id { get; set; }
            public string username { get; set; }
            public string displayname { get; set; }
            public int photo_id { get; set; }
            public string status { get; set; }
            public DateTime status_date { get; set; }
            public string locale { get; set; }
            public string language { get; set; }
            public string timezone { get; set; }
            public int show_profileviewers { get; set; }
            public int level_id { get; set; }
            public int invites_used { get; set; }
            public int extra_invites { get; set; }
            public int enabled { get; set; }
            public int verified { get; set; }
            public int approved { get; set; }
            public DateTime creation_date { get; set; }
            public DateTime modified_date { get; set; }
            public DateTime lastlogin_date { get; set; }
            public DateTime? update_date { get; set; }
            public int member_count { get; set; }
            public int view_count { get; set; }
            public int comment_count { get; set; }
            public int like_count { get; set; }
            public int coverphoto { get; set; }
            public string coverphotoparams { get; set; }
            public string view_privacy { get; set; }
            public int seao_locationid { get; set; }
            public string location { get; set; }
            public string friendship_type { get; set; }
            public string image { get; set; }
            public string image_normal { get; set; }
            public string image_profile { get; set; }
            public string image_icon { get; set; }
            public string content_url { get; set; }
            public int is_member_verified { get; set; }
            public MemberMenu menus = new MemberMenu()
            {
                label = "Add Friend",
                name = "add_friend",
                url = "user/add"//,
                //urlParams 
            };


            public Member(UserFull m)
            {
                user_id = (int)m.user_id;
                username = m.username;
                displayname = m.displayname;
                photo_id = (int)m.photo_id;
                status = m.status;
                status_date = m.status_date;
                locale = m.locale;
                language = m.language;
                timezone = m.timezone;
                show_profileviewers = m.show_profileviewers ? 1 : 0;
                level_id = (int)m.level_id;
                invites_used = (int)m.invites_used;
                extra_invites = (int)m.extra_invites;
                enabled = m.enabled ? 1 : 0;
                verified = m.verified ? 1 : 0;
                approved = m.approved ? 1 : 0;
                creation_date = m.creation_date;
                modified_date = m.modified_date;
                lastlogin_date = m.lastlogin_date;
                update_date = m.update_date;
                member_count = (int)m.member_count;
                view_count = (int)m.view_count;
                comment_count = (int)m.comment_count;
                like_count = (int)m.like_count;
                coverphoto = 0;
                coverphotoparams = m.coverphotoparams;
                view_privacy = m.view_privacy;
                seao_locationid = m.seao_locationid;
                location = m.location;
                friendship_type = "add_friend";
                if (photo_id != 0)
                {
                    FeedImage img = new FeedImage((uint)photo_id);
                    image = image_normal = image_profile = image_icon = img.src;
                }
                content_url = $"http://beta.koobeca.com/profile/{user_id}";
                is_member_verified = 0;//TODO

                menus.urlParams = new { user_id };
            }

            
        }
    }

    public class MemberMenu
    {
        public string label;
        public string name;
        public string url;
        public object urlParams;
    }
}