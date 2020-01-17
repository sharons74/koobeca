using KoobecaSync.APIs.Koobeca.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBSynch.APIs.Koobeca.Requests
{
    public class KoobecaGetFeedRequest : KoobecaApiRequest
    {
        public override string Endpoint { get; } = "advancedactivity/feeds";
    }
}
