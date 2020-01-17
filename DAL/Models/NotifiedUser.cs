using System;
using System.Collections.Generic;
using System.Text;

namespace KoobecaFeedController.DAL.Models
{
    /*
+-----------+---------------+------+-----+---------+----------------+
| Field     | Type          | Null | Key | Default | Extra          |
+-----------+---------------+------+-----+---------+----------------+
| action_id | int(11)       | NO   | PRI | NULL    | auto_increment |
| user_ids  | varchar(3000) | NO   |     | NULL    |                |
+-----------+---------------+------+-----+---------+----------------+
     */
    public class NotifiedUser
    {
        public int action_id { get; set; }
        public string user_ids { get; set; }

        public bool ContainsUser(uint userId)
        {
            if (string.IsNullOrEmpty(user_ids)) return false;
            if (user_ids == $"[{userId}]") return true;
            if (user_ids.StartsWith($"[{userId},")) return true;
            if (user_ids.EndsWith($",{userId}]")) return true;

            return user_ids.Contains($",{userId},");
        }

        public void AddUser(uint userId)
        {
            if (string.IsNullOrEmpty(user_ids) || user_ids == "[]")
            {
                user_ids = $"[{userId}]";
            }
            else
            {
                user_ids = user_ids.Replace("]", $",{userId}]");
            }
        }

        public void RemoveUser(uint userId)
        {
            if (string.IsNullOrEmpty(user_ids)) return;

            if (user_ids == $"[{userId}]") user_ids = null;
            else if (user_ids.StartsWith($"[{userId},")) user_ids = user_ids.Replace($"[{userId},", "[");
            else if (user_ids.EndsWith($",{userId}]")) user_ids = user_ids.Replace($",{userId}]", "]");
            else user_ids = user_ids.Replace($",{userId}", "");
        }
    }
}
