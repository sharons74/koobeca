using System;
using System.Collections.Generic;
using KoobecaFeedController.BL.Rendering;
using KoobecaFeedController.BL.Request;
using KoobecaFeedController.DAL.Adapters;
using Newtonsoft.Json;

namespace KoobecaFeedController.BL.Handlers
{
    internal class UserProfileHandler : ActivityHandler
    {
        public UserProfileHandler(uint userId, RequestParams reqParams) : base(userId, reqParams)
        {
        }

        public override void Execute()
        {
            var userFull = Users.GetFullById((int)userId);
            var gutterMenu = new CommentMenuItem()
            {
                name = "user_home_edit",
                label = "Edit My Profile",
                url = "members/edit/profile",
                urlParams = null
            };
            var profile_tabs = GetProfileTab();


            object res = new
            {
                gutterMenu = gutterMenu,
                profile_tabs = profile_tabs,
                response = userFull
            };

            Response = JsonConvert.SerializeObject(res);
        }

        private ProfileTabItem[] GetProfileTab()
        {
            List<ProfileTabItem> items = new List<ProfileTabItem>();

            items.Add(new ProfileTabItem()
            {
                label = "Updates",
                name = "update"
            });
            items.Add(new ProfileTabItem()
            {
                label = "About",
                name = "info"
            });
            items.Add(new ProfileTabItem()
            {
                label = "Friends",
                name = "friends",
                totalItemCount = 0
            });
            items.Add(new ProfileTabItem()
            {
                label = "Groups",
                name = "sitegroup",
                totalItemCount = 0
            });
            items.Add(new ProfileTabItem()
            {
                label = "Events",
                name = "event",
                totalItemCount = 0,
                isAdvancedModuleEnabled = true
            });
            items.Add(new ProfileTabItem()
            {
                label = "Albums",
                name = "album",
                totalItemCount = 0
            });
            items.Add(new ProfileTabItem()
            {
                label = "Videos",
                name = "video",
                totalItemCount = 0,
                isAdvancedModuleEnabled = true
            });
            items.Add(new ProfileTabItem()
            {
                label = "Channels",
                name = "channel",
                totalItemCount = 0,
                isAdvancedModuleEnabled = true
            });
            items.Add(new ProfileTabItem()
            {
                label = "Pages",
                name = "sitepage",
                totalItemCount = 0
            });

            return items.ToArray();
        }
    }
}