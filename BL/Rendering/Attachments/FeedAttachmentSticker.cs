using KoobecaFeedController.DAL.Adapters;
using KoobecaFeedController.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace KoobecaFeedController.BL.Rendering.Attachments
{
    public class FeedAttachmentSticker : FeedAttachment
    {
        public string title;
        public string body;
        public string attachment_type;
        public uint attachment_id;
        public string uri;
        public string album_id;
        public string photo_id;


        public FeedImage image_main;
        public uint mode;

        private readonly Sticker _sticker;

        //
        public FeedAttachmentSticker(ActivityAttachment attachment, ulong actionId)
        {
            _sticker = Stickers.GetById(attachment.id);
            attachment_id = attachment.id;
            attachment_type = "sitereaction_sticker";
            title = _sticker.title;
            
            uri = $"https://beta.koobeca.com/view.php?action_id={actionId}";
            image_main = _sticker.file_id > 0 ? new FeedImage(_sticker.file_id) : null;
            mode = (uint)(attachment.mode ? 1 : 0);
        }

    }
}
