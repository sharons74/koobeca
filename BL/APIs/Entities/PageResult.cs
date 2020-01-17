using FBSynch.APIs.Koobeca;
using System;
using System.Collections.Generic;
using System.Text;

namespace KoobecaFeedController.BL.APIs.Entities
{
    public class PageResult : KBResult
    {
        public Body body;


        public class Body
        {
            public int page_id;
            public KBPage response;
        }
    }
}
