using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FB2Koobeca.Entities
{
    public class PagePost
    {
        public string PageUrl { get; set; }
        public string FaecbookID { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public string DownloadLink { get; set; }
        public Stream Photo { get; set; }
      
    }
}
