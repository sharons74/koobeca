using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace KoobecaFeedController.BL.Rendering {
    public class FeedAttachmentActivity : FeedAttachment {
        public uint id;
        public FeedWrapper[] shared_post_data;
        public string title = "";
        public string body;
        public string attachment_type;
        public uint attachment_id;
        public string uri;
        public uint mode;

        private readonly Activity _activity;

        public FeedAttachmentActivity(Activity activity, uint id, ulong forSource, string sourceType,bool isIOS) {
            _activity = activity;
            this.id = activity.RawActivity.object_id; //TODO
            shared_post_data = new[] {new FeedWrapper(activity, forSource, sourceType,isIOS)};
            var innerFeed = shared_post_data[0].feed;
            body = innerFeed.feed_title;
            attachment_type = "activity_action";
            attachment_id = innerFeed.action_id;
            uri = "TODO";
        }

        public override List<string> TagList {
            get {
                var list = HashTagFinder.FindTags(title);
                list.AddRange(HashTagFinder.FindTags(body));
                foreach (var sharedPost in shared_post_data) list.AddRange(sharedPost.TagList);
                return list.GroupBy(test => test).Select(grp => grp.First()).ToList();
            }
        }

        [JsonIgnore]
        public override uint CommentCount => _activity.RawActivity.comment_count;

        [JsonIgnore]
        public override uint LikeCount => _activity.RawActivity.like_count;
    }
}