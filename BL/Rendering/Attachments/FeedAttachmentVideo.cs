using System.Collections.Generic;
using System.Web;
using KoobecaFeedController.DAL.Adapters;
using KoobecaFeedController.DAL.Models;
using Newtonsoft.Json;

namespace KoobecaFeedController.BL.Rendering {
    public class FeedAttachmentVideo : FeedAttachment {
        public uint attachment_id;
        public string attachment_type;
        public uint attachment_video_type;
        public string attachment_video_code;
        public string attachment_video_url;
        public string body;
        public string content_url;
        public FeedImage image_main;
        public uint mode;
        public string title;
        public string uri;

        private readonly Video _video;

        //
        public FeedAttachmentVideo(ActivityAttachment attachment, ulong actionId) {
            _video = Videos.GetById(attachment.id);
            attachment_id = attachment.id;
            attachment_video_type = _video.GetVIdeoType();
            attachment_type = "video";
            title = _video.title;
            body = HttpUtility.HtmlDecode(_video.description);
            attachment_video_url = _video.GetVideoUrl(ServiceUrlPrefixes.CouldFront);
            if (_video.type == "fb")
            {
                Dictionary<string, string> data = JsonConvert.DeserializeObject<Dictionary<string, string>>(_video.code);
                image_main = new FeedImage() { src = data["thumb"] , size = new FeedImageSize() { height = 300,width = 300} };
                attachment_video_code = "mp4";
            }
            else
            {
                attachment_video_code = _video.code;
                image_main = _video.photo_id > 0 ? new FeedImage(_video.photo_id) : null;
            }
            content_url = $"https://beta.koobeca.com/view.php?action_id={actionId}";
            //content_url = uri = $"https://beta.koobeca.com/videos/{_video.owner_id}/{_video.video_id}/video";
            
            mode = (uint) (attachment.mode ? 1 : 0);
            
        }

        public override bool HasPhoto => true;

        [JsonIgnore]
        public override uint CommentCount => _video?.comment_count ?? 0;

        [JsonIgnore]
        public override uint LikeCount => (uint) (_video?.like_count ?? 0);
    }
}