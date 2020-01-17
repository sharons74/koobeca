using KoobecaFeedController.BL.APIs.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FB2Koobeca.Entities
{
    public class PostResult : KBResult
    {
        
        public Body body;
        

        public class Body
        {
            public int page_id;
            public object response;
        }
    }
}
