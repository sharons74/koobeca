using KoobecaFeedController.BL.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace KoobecaFeedController.BL.Handlers
{
    public class HandlerFactory
    {
        public static ActivityHandler CreateHandler(string cmd, uint userId, RequestParams requestParams, Dictionary<string, object> additionalParams)
        {
            switch (cmd)
            {
                case "feed":
                    return new GetFeedHandler(userId, requestParams);
                case "post":
                    requestParams.action_id = (ulong)((long)additionalParams["action_id"]);
                    return new PostHandler(userId, requestParams);
                case "edit":
                    return new EditHandler(userId, requestParams);
                case "delete":
                    if (requestParams.comment_id != 0)
                    {
                        Logger.Instance.Debug($"removing comment {requestParams.comment_id}");
                        return new UnCommentHandler(userId, requestParams);
                    }
                    else
                    {
                       return new RemoveActivityHandler(userId, requestParams);
                    }
                case "like":
                    if (requestParams.comment_id == 0)
                    {
                        return new LikesHandler(userId, requestParams);
                    }
                    else
                    {
                        return new CommentLikeHandler(userId, requestParams);
                    }
                case "unlike":
                    if (requestParams.comment_id == 0)
                    {
                        return new UnlikeHandler(userId, requestParams);
                    }
                    else
                    {
                        return new CommentUnlikeHandler(userId, requestParams);
                    }
                case "comment":
                    return new CommentHandler(userId, requestParams, additionalParams);
                case "save":
                    return new SaveActivityHandler(userId, requestParams);
                case "updateCommentable":
                    return new UpdateCommentableHandler(userId, requestParams);
                case "updateSharable":
                    return new UpdateSharableHandler(userId, requestParams);
                case "share":
                    if (requestParams.action_id == 0) requestParams.action_id = requestParams.id; // in share the action id is in the id
                    return new ShareHandler(userId, requestParams);
                case "hide":
                    return new HideHandler(userId, requestParams);
                case "unhide":
                    return new UnHideHandler(userId, requestParams);
                case "notificationOnOff":
                    return new NotificationsOnOffHandler(userId, requestParams);
                case "follow":
                    return new FollowHandler(userId, requestParams, additionalParams);
                case "view":
                    return new ViewHandler(userId, new RequestParams(), additionalParams);
                case "likesComments":
                    return new LikesCommentsHandler(userId, requestParams);
                case "reply":
                    return new ReplyHandler(userId, new RequestParams(), additionalParams);
                case "feedPostMenus":
                    return new FeedPostMenuHandler(userId, requestParams);
                case "greetingManagement":
                    return new GreetingHandler(userId, requestParams);
                case "banner":
                    return new BannerHandler(userId, requestParams);
                case "membersBrowse":
                    return new MembersBrowseHandler(userId, requestParams);
                case "membersSearch":
                    return new MembersSearchHandler(userId, requestParams);
                case "addFriend":
                    return new AddFriendHandler(userId, requestParams);
                case "friendRemove":
                    return new FriendRemoveHandler(userId, requestParams);
                case "coregetsettings":
                    return new CoreGetSettingsHandler(userId, requestParams);
                case "userprofileindex":
                    return new UserProfileHandler(userId, requestParams);
                case "getdashboardmenus":
                    return new GetDashboardMenusHandler(userId,requestParams);
                case "geterrorpagecontent":
                    return new ErrorPageContentHandler(userId, requestParams);
                case "feedDecoration":
                    return new FeedDecorationHandler(userId, requestParams);
                case "coregetnewversion":
                    return new GetVersionHandler(userId, requestParams);
                case "activitynewupdates":
                    return new ActivityNewUpdatesHandler(userId, requestParams);
                case "storycreate":
                    return new StoryCreateHandler(userId, requestParams);
                case "getserversettings":
                    return new GetServerSettingsHandler(userId, requestParams);
                case "storybrowse":
                    return new StoryBrowseHandler(userId, requestParams);
                case "userlogin":
                    return new UserLoginHandler(userId, requestParams);
                case "activitynotifications":
                    return new ActivityNotificationsHandler(userId, requestParams);
                case "activityhide":
                    return new ActivityHideHandler(userId, requestParams); 
                default:
                    Console.WriteLine($"have no handler for command {cmd}");
                    return null;
            }
        }
    }
}
