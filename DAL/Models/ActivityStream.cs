using System;
using System.Collections.Generic;
using System.Text;

namespace KoobecaFeedController.DAL.Models
{
    /*
+--------------+------------------+------+-----+---------+-------+
| Field        | Type             | Null | Key | Default | Extra |
+--------------+------------------+------+-----+---------+-------+
| target_type  | varchar(32)      | NO   | PRI | NULL    |       |
| target_id    | int (11) unsigned| NO   | PRI | NULL    |       |
| subject_type | varchar(24)      | NO   | MUL | NULL    |       |
| subject_id   | int (11) unsigned| NO   |     | NULL    |       |
| object_type  | varchar(24)      | NO   | MUL | NULL    |       |
| object_id    | int (11) unsigned| NO   |     | NULL    |       |
| type         | varchar(64)      | NO   |     | NULL    |       |
| action_id    | int (11) unsigned| NO   | PRI | NULL    |       |
+--------------+------------------+------+-----+---------+-------+
*/
    public class ActivityStream
    {
        public string target_type { get; set; }
        public uint target_id { get; set; }
        public string subject_type { get; set; }
        public uint subject_id { get; set; }
        public string object_type { get; set; }
        public uint object_id { get; set; }
        public string type { get; set; }
        public uint action_id { get; set; }

        public static ActivityStream Create(ActivityAction action,string targetType,ulong targetId = 0)
        {
            ActivityStream activityStream = new ActivityStream();
            activityStream.target_type = targetType;
            activityStream.target_id = (uint)targetId;
            activityStream.subject_id = action.subject_id;
            activityStream.subject_type = action.subject_type;
            activityStream.object_id = action.object_id;
            activityStream.object_type = action.object_type;
            activityStream.type = action.type;
            activityStream.action_id = action.action_id;
            return activityStream;
        }
    }

    
}
