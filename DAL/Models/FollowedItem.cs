using System;
using System.Collections.Generic;
using System.Text;

namespace KoobecaFeedController.DAL.Models
{
    /*
     MariaDB [koobdb]> desc engine4_seaocore_follows;
+---------------+------------------+------+-----+---------+----------------+
| Field         | Type             | Null | Key | Default | Extra          |
+---------------+------------------+------+-----+---------+----------------+
| follow_id     | int(11) unsigned | NO   | PRI | NULL    | auto_increment |
| resource_type | varchar(32)      | NO   | MUL | NULL    |                |
| resource_id   | int(11) unsigned | NO   |     | NULL    |                |
| poster_type   | varchar(32)      | NO   | MUL | NULL    |                |
| poster_id     | int(11) unsigned | NO   |     | NULL    |                |
| creation_date | datetime         | NO   |     | NULL    |                |
+---------------+------------------+------+-----+---------+----------------+
     */
    public class FollowedItem
    {
        public uint follow_id { get; set; }
        public string resource_type { get; set; }
        public uint resource_id { get; set; }
        public string poster_type { get; set; }
        public uint poster_id { get; set; }
        public DateTime creation_date { get; set; }
    }
}
