using System;
using System.Collections.Generic;
using System.Text;

namespace KoobecaFeedController.DAL.Models
{
    /*
+-------------+---------------------+------+-----+---------+-------+
| Field       | Type                | Null | Key | Default | Extra |
+-------------+---------------------+------+-----+---------+-------+
| page_id     | bigint(20) unsigned | NO   |     | NULL    |       |
| source_type | varchar(24)         | NO   |     | NULL    |       |
| source_id   | varchar(100)        | NO   |     | NULL    |       |
| source_name | varchar(100)        | YES  |     | NULL    |       |
+-------------+---------------------+------+-----+---------+-------+
     */
    public class PageSource
    {
        public ulong page_id { get; set; }
        public string source_type { get; set; }
        public string source_id { get; set; }
        public string source_name { get; set; }
    }
}
