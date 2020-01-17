using System;
using System.Collections.Generic;
using System.Text;

namespace KoobecaFeedController.BL.APIs.Entities
{
    public class VideoResult : KBResult
    {
        public Body body;


        public class Body
        {
            public KBVideo response;
        }
    }
}
