using KoobecaFeedController.BL.Request;
using KoobecaFeedController.DAL.Adapters;
using Newtonsoft.Json;
using System.Linq;

namespace KoobecaFeedController.BL.Handlers
{
    public class BannerHandler : ActivityHandler
    {
        public BannerHandler(uint userId, RequestParams reqParams) : base(userId, reqParams)
        {
        }

        public override void Execute()
        {
            var responseList = Banners.GetAll().Where(b=>b.file_id > 0).Select(b => new BannerVM
            {
               image = $"url({ServiceUrlPrefixes.CouldFront + Storages.GetByFileId((uint)b.file_id).storage_path})",
               feed_banner_url = $"{ServiceUrlPrefixes.CouldFront + Storages.GetByFileId((uint)b.file_id).storage_path}",
               background_color = b.background_color,
               color = b.color,
               highlighted = b.highlighted
            }).ToList();

            Response = JsonConvert.SerializeObject(responseList);
         }

        public class BannerVM
        {
            public string image { get; set; }
            public string feed_banner_url { get; set; }
            [JsonProperty(PropertyName = "background-color")]
            public string background_color { get; set; }
            public string color { get; set; }
            public sbyte highlighted { get; set; }
        }
    }
}