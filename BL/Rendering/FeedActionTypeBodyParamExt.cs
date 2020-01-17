using Newtonsoft.Json;

namespace KoobecaFeedController.BL.Rendering {
    public class FeedActionTypeBodyParamExt : FeedActionTypeBodyParam {
        [JsonProperty(Required = Required.Default)]
        public string type;

        [JsonProperty(Required = Required.Default)]
        public ulong id;
    }
}