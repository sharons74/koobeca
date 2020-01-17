using KoobecaFeedController.BL.Request;
using KoobecaFeedController.DAL.Adapters;
using KoobecaFeedController.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KoobecaFeedController.BL.Handlers
{
    public class UnCommentHandler : RemoveRefHandler
    {
        public UnCommentHandler(uint userId,RequestParams reqParams) : base(userId,reqParams, ActivityType.Comment) { }

        protected override void DecreaseCount()
        {
            activity.RawActivity.comment_count--;
        }

        protected override void RemoveRecordFromTable(object refRecord)
        {
            Comment comment = refRecord as Comment;
            Comments.Remove(comment.comment_id);
            if(!string.IsNullOrEmpty(comment.attachment_type) && comment.attachment_id != 0)
            {
                uint fileId = 0;
                //remove attachment
                if(comment.attachment_type == "album_photo")
                {
                    var photo = AlbumPhotos.GetById((uint)comment.attachment_id);
                    fileId = photo.file_id;
                }
                  
                if(fileId != 0)
                {
                    Logger.Instance.Debug($"Removing storage file {fileId}");
                    Storages.DeleteByFileId(fileId);
                }
            }
        }

        protected override void CheckIfRefNotExists(out ActivityAction refActivityAction, out object actionRecord)
        {
            Logger.Instance.Debug($"Comment id = {reqParams.comment_id}");
            var comment = Comments.GetByCommentId(reqParams.comment_id);
            actionRecord = comment ?? throw new Exception($"Comment to {activity.ActivityId} by user {userId} not exists in Comments table");

            var allComments = ActivityActions.GetByObjectId((uint)activity.ActivityId, "activity_actions");
            foreach(var aa in allComments)
            {
                Logger.Instance.Debug($"found {aa.type} , id {aa.action_id} on time {aa.modified_date}");
            }
            Logger.Instance.Debug($"looking to match comment on {activity.ActivityId} with date {comment.creation_date}");

            refActivityAction = ActivityActions.GetRefActivity("comment_activity_action", activity.ActivityId, "activity_action", userId, false, comment.creation_date);
            
            if (refActivityAction == null)
            {
                Logger.Instance.Warn($"Inconsistancy in Comments and activity tables , to {activity.ActivityId} by user {userId}");
                //throw new Exception($"Inconsistancy in Comments and activity tables , to {activity.ActivityId} by user {userId}");
            }
        }
    }
}
