using KoobecaSync.APIs.Koobeca;
using KoobecaSync.APIs.Koobeca.Requests;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace KoobecaFeedController.BL.APIs.Koobeca.Requests
{
    
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    class KoobecaCreateVideoRequest : KoobecaApiRequest
    {
        public override string Endpoint { get; } = "advancedvideos/create";
        [KoobecaParam]
        public string title { get; set; }
        [KoobecaParam]
        public string description { get; set; }
        [KoobecaParam]
        public string type { get; set; }
        [KoobecaParam]
        public string url { get; set; }
        [KoobecaParam]
        public int package_id { get; set; }
        [KoobecaParam]
        public int category_id { get; set; } = 4;
        [KoobecaParam]
        public int subcategory_id { get; set; } = 1;
        [KoobecaParam]
        public int subsubcategory_id { get; set; } = 19;
        [KoobecaParam]
        public string auth_comment { get; set; } = "onlyme";
    }
}
