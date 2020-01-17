using System;
using System.Linq;
using KoobecaFeedController.BL.Rendering.Attachments;
using KoobecaFeedController.DAL.Adapters;
using KoobecaFeedController.DAL.Models;

namespace KoobecaFeedController.BL.Rendering
{
    public class CommentAttachment
    {
        public string image_profile;
        public FeedImageSize size;
        private FeedAttachment feedAttachment;

        public CommentAttachment(FeedAttachment feedAttachment)
        {
            this.feedAttachment = feedAttachment;
            if(feedAttachment is FeedAttachmentPhoto)
            {
                var photoAttachment = feedAttachment as FeedAttachmentPhoto;
                image_profile = photoAttachment.image_main.src;
                size = photoAttachment.image_main.size;
            }
            else if (feedAttachment is FeedAttachmentSticker)
            {
                var stickerAttachment = feedAttachment as FeedAttachmentSticker;
                image_profile = stickerAttachment.image_main.src;
                size = stickerAttachment.image_main.size;
            }
        }

        public static CommentAttachment Create(Comment comment, ulong action_id)
        {
            CommentAttachment attachment = null;
            if (!string.IsNullOrEmpty(comment.attachment_type)) {
                ActivityAttachment attach = new ActivityAttachment() { id = (uint)comment.attachment_id, type = comment.attachment_type, mode = true };
                //var attach = ActivityAttachments.GetByTypeAndId((comment.attachment_type,(uint)comment.attachment_id)).FirstOrDefault();
                var feedAttachment = FeedAttachmentFactory.Create(attach,action_id);
                attachment = new CommentAttachment(feedAttachment);
            }

            return attachment;
        }
    }
}