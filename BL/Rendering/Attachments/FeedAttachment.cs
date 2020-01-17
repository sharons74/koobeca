using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace KoobecaFeedController.BL.Rendering {
    public class FeedAttachment {
        [JsonIgnore]
        public virtual bool HasPhoto => false;

        [JsonIgnore]
        public virtual List<string> TagList => new List<string>();

        [JsonIgnore]
        public virtual uint CommentCount => throw new NotImplementedException();

        [JsonIgnore]
        public virtual uint LikeCount => throw new NotImplementedException();
    }
}