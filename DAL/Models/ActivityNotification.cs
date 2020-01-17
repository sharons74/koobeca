using System;
using System.Collections.Generic;
using System.Text;

namespace KoobecaFeedController.DAL.Models
{
    /*
+-----------------+------------------+------+-----+---------+----------------+
| Field           | Type             | Null | Key | Default | Extra          |
+-----------------+------------------+------+-----+---------+----------------+
| notification_id | int(11) unsigned | NO   | PRI | NULL    | auto_increment |
| user_id         | int(11) unsigned | NO   | MUL | NULL    |                |
| subject_type    | varchar(64)      | NO   | MUL | NULL    |                |
| subject_id      | int (11) unsigned| NO   |     | NULL    |                |
| object_type     | varchar(64)      | NO   | MUL | NULL    |                |
| object_id       | int (11) unsigned| NO   |     | NULL    |                |
| type            | varchar(64)      | NO   |     | NULL    |                |
| params          | text             | YES  |     | NULL    |                |
| read            | tinyint(1)       | NO   |     | 0       |                |
| mitigated       | tinyint(1)       | NO   |     | 0       |                |
| date            | datetime         | NO   |     | NULL    |                |
| show            | tinyint(1)       | NO   |     | 0       |                |
+-----------------+------------------+------+-----+---------+----------------+
*/
    public class ActivityNotification
    {
        public uint notification_id { get; set; }
        public uint user_id { get; set; }
        public string subject_type { get; set; }
        public uint subject_id { get; set; }
        public string object_type { get; set; }
        public uint object_id { get; set; }
        public string type { get; set; }
        public string @params { get; set; }
        public bool read { get; set; }
        public bool mitigated { get; set; }
        public DateTime date { get; set; }
        public bool show { get; set; }
    }
}
