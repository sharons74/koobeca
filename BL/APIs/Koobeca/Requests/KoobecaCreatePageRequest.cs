using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace KoobecaSync.APIs.Koobeca.Requests {
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    class KoobecaCreatePageRequest : KoobecaApiRequest {
        public override string Endpoint { get; } = "sitepages/create";
        [KoobecaParam]
        public string sitepage { get; set; }
        [KoobecaParam]
        public string title { get; set; }
        [KoobecaParam]
        public string page_url { get; set; }
        [KoobecaParam]
        public string location { get; set; }
        [KoobecaParam]
        public int package_id { get; set; }
        [KoobecaParam]
        public int category_id { get; set; }
        [KoobecaParam]
        public string auth_comment { get; set; }
    }
}
