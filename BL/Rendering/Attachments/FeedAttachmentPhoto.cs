using System.Net;
using System.Web;
using KoobecaFeedController.DAL.Adapters;
using KoobecaFeedController.DAL.Models;
using Newtonsoft.Json;

namespace KoobecaFeedController.BL.Rendering {
    public class FeedAttachmentPhoto : FeedAttachment {
        public string title;
        public string body;
        public string attachment_type;
        public uint attachment_id;
        public string uri;
        public string album_id;
        public string photo_id;


        public FeedImage image_main;
        public uint mode;

        private readonly AlbumPhoto _photo;

        //
        public FeedAttachmentPhoto(ActivityAttachment attachment, ulong actionId) {
            _photo = AlbumPhotos.GetById(attachment.id);
            attachment_id = attachment.id;
            attachment_type = "album_photo";
            title = _photo.title;
            body = HttpUtility.HtmlDecode(_photo.description);

            //uri = $"https://beta.koobeca.com/advancedalbums/photo/view/album_id/{_photo.album_id}/photo_id/{_photo.photo_id}";
            //uri = $"https://beta.koobeca.com/core/link/index/action_id/{actionId}";
            uri = $"https://beta.koobeca.com/view.php?action_id={actionId}";
            image_main = _photo.file_id > 0 ? new FeedImage(_photo.file_id) : null;
            mode = (uint) (attachment.mode ? 1 : 0);
        }

        public override bool HasPhoto => true;

        [JsonIgnore]
        public override uint CommentCount => _photo?.comment_count ?? 0;

        [JsonIgnore]
        public override uint LikeCount => _photo?.like_count ?? 0;
    }
}