using System.Linq;
using System.Net;
using KoobecaFeedController.DAL.Adapters;
using KoobecaFeedController.DAL.Models;
using Newtonsoft.Json;

namespace KoobecaFeedController.BL.Rendering {
    public class FeedImage {
        public string src;
        public FeedImageSize size;

        public FeedImage()
        {
        }

        public FeedImage(uint photoId) {
            var storage = Storages.GetByFileId(photoId);
            src = ServiceUrlPrefixes.CouldFront + storage.storage_path;
            size = JsonConvert.DeserializeObject<FeedImageSize>(storage.@params);
        }

        public FeedImage(Framely iframely) {
            var thumb = iframely.links?.thumbnail?.Where(t => (t?.media?.width??1000) < 400).FirstOrDefault();
            if (thumb != null) {
                src = thumb.href;

                size = new FeedImageSize {height = thumb.media.height, width = thumb.media.width};
            }
        }
    }
}