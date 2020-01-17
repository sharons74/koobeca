using KoobecaFeedController.DAL.Adapters;
using KoobecaFeedController.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KoobecaFeedController.BL.Rendering
{
    public class CommentInfo
    {
        public ulong action_id;
        public ulong comment_id;
        public string image;
        public string image_normal;
        public string image_profile;
        public string image_icon;
        public string content_url;
        public string author_image;
        public string author_image_normal;
        public string author_image_profile;
        public string author_image_icon;
        public string author_title;
        public string @params;
        public ulong user_id;
        public string userTag;
        public uint like_count; 
        public string comment_date;//2019-01-10 19:02:18
        public CommentMenuItem[] gutterMenu;
        public string comment_body;
        public CommentAttachment attachment;
        public string attachment_type;
        public ulong attachment_id;
        //public CommentMenuItem delete;
        public CommentMenuItem like;
        public CommentMenuItem reply;
        //public CommentMenuItem edit;
        
        public int isReply = 0;
        public CommentInfo[] reply_on_comment;
        public int reply_count;


        public CommentInfo(Comment comment, ulong viewerId, bool? isLike = false,List<Comment> children = null,bool isReply = false)
        {

            Logger.Instance.Debug($"building comment info for comment {comment.comment_id}");
            var user = Users.GetById(comment.poster_id);
            action_id = comment.resource_id;
            comment_id = comment.comment_id;
            image = comment.GetParam<string>("image");
            image_normal = comment.GetParam<string>("image_normal");
            image_profile = comment.GetParam<string>("image_profile");
            image_icon = comment.GetParam<string>("image_icon");
            content_url = comment.GetParam<string>("content_url");
            author_image = comment.GetParam<string>("author_image");
            author_image_normal = comment.GetParam<string>("author_image_normal");
            author_image_profile = comment.GetParam<string>("author_image_profile");
            author_image_icon = comment.GetParam<string>("author_image_icon");
            author_title = user.displayname;
            comment_body = Encoding.UTF8.GetString(comment.body);
            user_id = comment.poster_id;
            userTag = string.Empty;
            //    //@params;
            comment_date = comment.creation_date.ToString("yyyy-MM-dd HH:mm:ss");
            attachment = CommentAttachment.Create(comment, action_id);

            attachment_type = comment.attachment_type;
            attachment_id = (ulong)comment.attachment_id;

            //build gutter menu
            List<CommentMenuItem> menu = new List<CommentMenuItem>();
            if (viewerId == comment.poster_id)
            {
                menu.Add(new CommentMenuItem("comment_delete", comment));
                menu.Add(new CommentMenuItem("comment_edit", comment));
            }
            menu.Add(new CommentMenuItem("comment_copy", comment));
            menu.Add(new CommentMenuItem("comment_cancel", comment));
            gutterMenu = menu.ToArray();

            if (!isReply)
            { 
                if (children != null)
                {
                    List<CommentInfo> subCumments = new List<CommentInfo>();
                    children.ForEach(c => subCumments.Add(new CommentInfo(c, viewerId, null, null, true)));
                    reply_on_comment = subCumments.OrderByDescending(c => c.comment_id).ToArray();
                    reply_count = reply_on_comment.Length;
                }
                reply = new CommentMenuItem("reply", comment);
            }

            this.isReply = isReply?0:1;

            if (!isLike.HasValue)
            {
                //find if user already like this commewnt
                isLike =  Likes.GetByResourceAndPoster(comment.comment_id, "activity_comment", (uint)user_id, "user") != null;
            }

            
            if (isLike == true)
            {
                like = new CommentMenuItem("unlike", comment);
            }
            else
            {
                like = new CommentMenuItem("like", comment);
            }

            like_count = (uint)CoreLikes.CountByResource(comment.comment_id, "activity_comment").Count();
        }
    }
}
