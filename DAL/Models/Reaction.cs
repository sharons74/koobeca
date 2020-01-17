using System;
using System.Collections.Generic;
using System.Text;

namespace KoobecaFeedController.DAL.Models
{
    /*
+-----------------+------------------+------+-----+---------+----------------+
| Field           | Type             | Null | Key | Default | Extra          |
+-----------------+------------------+------+-----+---------+----------------+
| reactionicon_id | int(11) unsigned | NO   | PRI | NULL    | auto_increment |
| title           | varchar(128)     | NO   |     | NULL    |                |
| type            | varchar(128)     | NO   |     | NULL    |                |
| photo_id        | int(10) unsigned | NO   |     | 0       |                |
| order           | int(11) unsigned | NO   |     | 0       |                |
+-----------------+------------------+------+-----+---------+----------------+
     */
    public class Reaction
    {
        public uint reactionicon_id { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public uint photo_id { get; set; }
        public uint order { get; set; }
    }
}
