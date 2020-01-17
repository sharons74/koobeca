using KoobecaFeedController.DAL.Adapters;
using KoobecaFeedController.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KoobecaFeedController.BL.Rendering.Comments
{
    public class LikesCommentsBody
    {
        public LikeBy[] viewAllLikesBy;
        public CommentInfo[] viewAllComments;
        public uint? isLike;
        public uint? canComment;
        public uint? canDelete;
        public int getTotalComments;
        public int getTotalItemCount;
        public int getTotalLikes;

        private Activity activity;
        private ulong commentId;

        public static LikesCommentsBody Create(Activity activity,ulong userId,bool viewAllComments, ulong comment_id,int ios)
        {
            try
            {
                return new LikesCommentsBody(activity,userId, viewAllComments, comment_id,ios);
            }
            catch(Exception e)
            {
                Logger.Instance.Warn("Failed to render likes/comments body");
                Logger.Instance.Debug(e.ToString());
                return null;
            }
        }


        private LikesCommentsBody(Activity activity,ulong userId, bool viewComments, ulong comment_id, int ios)
        {
            this.activity = activity;
            this.commentId = comment_id;

            if(viewComments)
            {
                viewAllComments = GetComments(userId,commentId,out getTotalComments,ios);
            }
           // else
          //  {
                viewAllLikesBy = GetLikes();
           // }

            var wrapper = FeedWrapper.Create(activity, userId, "user", false);
            isLike = wrapper.is_like;
            canComment = wrapper.canComment;
            canDelete = canComment;
            getTotalItemCount = viewAllComments == null ? 0 : viewAllComments.Length;
            getTotalLikes = viewAllLikesBy == null ? 0 : viewAllLikesBy.Length;
        }

        private LikeBy[] GetLikes()
        {
            List<LikeBy> likes = new List<LikeBy>();
            var userLikes = Likes.GetByResourceId((uint)activity.ActivityId).Where(l => l.poster_type == "user");
            foreach(var like in userLikes)
            {
                likes.Add(new LikeBy(like));
            }
            return likes.ToArray();
        }

        private CommentInfo[] GetComments(ulong viewerId,ulong commentId,out int totalComments, int ios)
        {
            List<CommentInfo> res = new List<CommentInfo>();
            Comment[] allComments = null;
            if (commentId == 0)
            {
                allComments = DAL.Adapters.Comments.GetForAction(activity.ActivityId);
            }
            else
            {
                allComments = DAL.Adapters.Comments.GetForComment(commentId);
            }

            //var allComments = DAL.Adapters.Comments.GetForAction(activity.ActivityId);
            totalComments = allComments.Count();
            var parentComments = allComments.Where(c => c.parent_comment_id == 0).OrderByDescending(c=>c.comment_id).ToList();
            Dictionary<int, List<Comment>> childCommentMap = new Dictionary<int, List<Comment>>();
            parentComments.ForEach(c => childCommentMap[(int)c.comment_id] = new List<Comment>()); //initialize map
            allComments.Where(c => c.parent_comment_id != 0).ToList().
                ForEach(c => childCommentMap[c.parent_comment_id].Add(c)); //set children in the map
            foreach (var comment in parentComments)
            {
                res.Add(new CommentInfo(comment,viewerId,null,childCommentMap[(int)comment.comment_id]));
            }

            if(ios == 1)
            {
                Logger.Instance.Debug("Flattening replies for iOS");
                List<CommentInfo> newRes = new List<CommentInfo>();
                foreach(var comment in res)
                {
                    newRes.Add(comment);
                    if(comment.reply_on_comment != null && comment.reply_on_comment.Length > 0)
                    {
                        var replies = comment.reply_on_comment;
                        comment.reply_on_comment = null;
                        foreach(var reply in replies)
                        {
                            reply.comment_body = $"[{comment.author_title}] {reply.comment_body}";
                            newRes.Add(reply);
                        }
                    }
                }
                return newRes.ToArray();
            }
            
            return res.ToArray();
        }
    }
}
