using System;
using System.Collections.Generic;
using System.Text;
using KoobecaFeedController.BL.Request;
using KoobecaFeedController.DAL.Adapters;

namespace KoobecaFeedController.BL.Handlers
{
    public class CommentUnlikeHandler : ActivityHandler
    {
        public CommentUnlikeHandler(uint userId, RequestParams reqParams) : base(userId, reqParams)
        {
        }

        public override void Execute()
        {
            Logger.Instance.Debug($"handling unlike of comment {reqParams.comment_id}");
            //add like count in comment
            var comment = Comments.GetByCommentId(reqParams.comment_id);
            comment.like_count--;
            Comments.UpdateLikeCount(comment.comment_id, comment.like_count);
            
            var rowcount = CoreLikes.RemoveCommentLike(reqParams.comment_id,userId);
            if (rowcount > 0) Logger.Instance.Debug($"Removed {rowcount} rows of like");
        }
    }
}
