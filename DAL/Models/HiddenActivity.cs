using System;
using System.Collections.Generic;
using System.Text;

namespace KoobecaFeedController.DAL.Models
{
    /*
+--------------------+------------------+------+-----+---------+-------+
| Field              | Type             | Null | Key | Default | Extra |
+--------------------+------------------+------+-----+---------+-------+
| user_id            | int(11) unsigned | NO   | PRI | NULL    |       |
| hide_resource_type | varchar(100)     | NO   | PRI | NULL    |       |
| hide_resource_id   | int(11) unsigned | NO   | PRI | NULL    |       |
+--------------------+------------------+------+-----+---------+-------+
     */
    public class HiddenActivity
    {
        public uint user_id { get; set; }
        public string hide_resource_type { get; set; } = "activity_action";
        public uint hide_resource_id { get; set; }
    }
}
