using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace KoobecaSync.APIs.Koobeca.Requests {
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    class KoobecaPostRequest : KoobecaApiRequest {

        public override string Endpoint { get; } = "advancedactivity/feeds/post";

        [KoobecaParam]
        public string type { get; set; } = "";

        [KoobecaParam]
        public int video_id { get; set; }

        [KoobecaParam]
        public string uri { get; set; } = "";

        [KoobecaParam]
        public string subject_type { get; set; } = "user";

        [KoobecaParam]
        public int subject_id { get; set; } = 1;

        [KoobecaParam]
        public string notifyItemAt { get; set; } = "1";

        [KoobecaParam]
        public int post_attach { get; set; } = 1;

    }

    
}
