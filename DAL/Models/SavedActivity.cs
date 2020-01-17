using System;
using System.Collections.Generic;
using System.Text;

namespace KoobecaFeedController.DAL.Models
{
    /*
+-------------+--------------+------+-----+---------+-------+
| Field       | Type         | Null | Key | Default | Extra |
+-------------+--------------+------+-----+---------+-------+
| user_id     | int(11)      | NO   | PRI | NULL    |       |
| action_type | varchar(128) | NO   |     | NULL    |       |
| action_id   | int(11)      | NO   | PRI | NULL    |       |
+-------------+--------------+------+-----+---------+-------+
     */
    public class SavedActivity
    {
        public int user_id { get; set; }
        public string action_type { get; set; }
        public int action_id { get; set; }
    }
}
