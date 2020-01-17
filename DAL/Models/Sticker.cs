using System;
using System.Collections.Generic;
using System.Text;

namespace KoobecaFeedController.DAL.Models
{
/*
+---------------+------------------+------+-----+---------+----------------+
| Field         | Type             | Null | Key | Default | Extra          |
+---------------+------------------+------+-----+---------+----------------+
| sticker_id    | int(11) unsigned | NO   | PRI | NULL    | auto_increment |
| title         | varchar(128)     | YES  |     | NULL    |                |
| collection_id | int(11) unsigned | NO   |     | NULL    |                |
| file_id       | int(10) unsigned | NO   |     | 0       |                |
| order         | int(11) unsigned | NO   |     | 999     |                |
+---------------+------------------+------+-----+---------+----------------+
*/
    public class Sticker
    {
        public uint sticker_id { set; get; }
        public string title { set; get; }
        public uint collection_id { set; get; }
        public uint file_id { set; get; }
        public uint order { set; get; }
    }
}
