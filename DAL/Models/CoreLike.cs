using System;

namespace KoobecaFeedController.DAL.Models {
    /*  
  +---------------+------------------+------+-----+---------+----------------+
  | Field         | Type             | Null | Key | Default | Extra          |
  +---------------+------------------+------+-----+---------+----------------+
  | like_id       | int(11) unsigned | NO   | PRI | NULL    | auto_increment |
  | resource_type | varchar(32)      | NO   | MUL | NULL    |                |
  | resource_id   | int (11) unsigned| NO   |     | NULL    |                |
  | poster_type   | varchar(32)      | NO   | MUL | NULL    |                |
  | poster_id     | int (11) unsigned| NO   |     | NULL    |                |
  | reaction      | varchar(64)      | YES  | MUL | like    |                |
  | creation_date | datetime         | NO   |     | NULL    |                |
  +---------------+------------------+------+-----+---------+----------------+
  */
    public class CoreLike : Like {
        public uint like_id { get; set; }
        public string resource_type { get; set; }
        public uint resource_id { get; set; }
        public string poster_type { get; set; }
        public uint poster_id { get; set; }
        public string reaction { get; set; }
        public DateTime creation_date { get; set; }
    }
}