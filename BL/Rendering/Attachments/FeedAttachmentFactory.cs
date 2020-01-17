using KoobecaFeedController.BL.Rendering.Attachments;
using KoobecaFeedController.DAL.Models;

namespace KoobecaFeedController.BL.Rendering {
    public class FeedAttachmentFactory {
        public static FeedAttachment Create(ActivityAttachment attachment,ulong actionId) {
            switch (attachment.type) {
                case "video":
                    return new FeedAttachmentVideo(attachment,actionId);
                case "core_link":
                    return new FeedAttachmentCore(attachment,actionId);
                case "album_photo":
                    return new FeedAttachmentPhoto(attachment,actionId);
                case "sitereaction_sticker":
                    return new FeedAttachmentSticker(attachment, actionId);
                default:
                    return null; // new FeedAttachment(attachment);
            }
        }
    }
}