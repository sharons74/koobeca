using System;
using Newtonsoft.Json;

namespace KoobecaFeedController.DAL.Models {
    /*
     *+---------------+-----------------------+------+-----+---------+----------------+
    | Field         | Type                  | Null | Key | Default | Extra          |
    +---------------+-----------------------+------+-----+---------+----------------+
    | link_id       | int(11) unsigned      | NO   | PRI | NULL    | auto_increment |
    | uri           | varchar(255)          | NO   |     | NULL    |                |
    | title         | varchar(255)          | NO   |     | NULL    |                |
    | description   | text                  | NO   |     | NULL    |                |
    | photo_id      | int(11) unsigned      | NO   |     | 0       |                |
    | parent_type   | varchar(24)           | NO   | MUL | NULL    |                |
    | parent_id     | int(11) unsigned      | NO   |     | NULL    |                |
    | owner_type    | varchar(24)           | NO   | MUL | NULL    |                |
    | owner_id      | int(11) unsigned      | NO   |     | NULL    |                |
    | view_count    | mediumint(6) unsigned | NO   |     | 0       |                |
    | creation_date | datetime              | NO   |     | NULL    |                |
    | search        | tinyint(1)            | NO   |     | 1       |                |
    | params        | text                  | YES  |     | NULL    |                |
    +---------------+-----------------------+------+-----+---------+----------------+ 
     */

    public class CoreLink : IActivityObject {
        public uint link_id { set; get; }
        public string uri { set; get; }
        public string title { set; get; }
        public string description { set; get; }
        public uint photo_id { set; get; }
        public string parent_type { set; get; }
        public uint parent_id { set; get; }
        public string owner_type { set; get; }
        public uint owner_id { set; get; }
        public uint view_count { set; get; }
        public DateTime creation_date { set; get; }
        public bool search { set; get; }
        public string @params { set; get; }

        string IActivityObject.GetObjectTitle() {
            return title;
        }

        uint IActivityObject.GetStorageFileId() {
            return photo_id;
        }

        public Framely GetIFramely() {
            if (!string.IsNullOrEmpty(@params) && @params.Contains("iframely")) {
                var p = JsonConvert.DeserializeObject<Parameters>(@params);
                return p.iframely;
            }

            return null;
        }

        public string GetObjectDesctiption()
        {
            return description;
        }

        public class Parameters {
            public Framely iframely;
        }

        
    }

    public class Framely
    {
        public string url;
        public Meta meta;
        public Links links;

        public class Links
        {
            public Thumbnail[] thumbnail;
            public Player[] player;
            public class Thumbnail
            {
                public Size media;
                public string href;

                public class Size
                {
                    public uint width;
                    public uint height;
                }
            }

            public class Player
            {
                public string type;
                public string html;
            }
        }

        public class Meta
        {
            public string media;
            public string description;
            public string title;
            public string site;
            public string canonical;
            public string author_url;
            public string author;
        }
    }
}