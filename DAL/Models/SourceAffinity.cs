using System;
using System.Collections.Generic;
using System.Text;

namespace KoobecaFeedController.DAL.Models
{
    /*
+-------------+---------------------+------+-----+---------+-------+
| Field       | Type                | Null | Key | Default | Extra |
+-------------+---------------------+------+-----+---------+-------+
| record_id   | bigint(20) unsigned | NO   | PRI | NULL    |       |
| user_id     | bigint(20) unsigned | NO   |     | NULL    |       |
| source_id   | bigint(20) unsigned | NO   |     | NULL    |       |
| source_type | varchar(24)         | NO   |     | NULL    |       |
| affinity    | tinyint(3) unsigned | NO   |     | NULL    |       |
| date        | datetime            | NO   |     | NULL    |       |
+-------------+---------------------+------+-----+---------+-------+
     */
    public class SourceAffinity
    {
        public ulong record_id { get; set; }
        public ulong user_id { get; set; }
        public ulong source_id { get; set; }
        public string source_type { get; set; }
        public byte affinity { get; set; }
        public DateTime date { get; set; }
    }
}
