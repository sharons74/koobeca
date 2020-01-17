using KoobecaFeedController.DAL.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KoobecaFeedController.DAL.Adapters
{
    public class NotifiedUsers
    {
        public static List<ulong> Get(ulong action_id)
        {
            var record = GetRecord(action_id);
            if (record == null) return new List<ulong>();

            var list = record.user_ids.Split(',');
            list[0] = list[0].Replace("[", "");
            list[list.Length - 1] = list[list.Length - 1].Replace("]", "");
            return list.Where(u => !string.IsNullOrEmpty(u)).Select(u => ulong.Parse(u)).ToList();
            
        }

        public static NotifiedUser GetRecord(ulong action_id)
        {
            return DbUtils.ConnectAndGet(conn =>
            {
                var query = "SELECT * FROM engine4_advancedactivity_notificationsettings WHERE action_id=@action_id";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("action_id", action_id);
                return DbUtils.GetRecords<NotifiedUser>(cmd);
            }).FirstOrDefault();
        }

        public static ulong Add(NotifiedUser newRecord)
        {
            return DbUtils.ConnectAndExecute(conn => {
                var query = "INSERT INTO engine4_advancedactivity_notificationsettings(action_id,user_ids) " +
                                                       "VALUES (@action_id,@user_ids)";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("action_id", newRecord.action_id);
                cmd.Parameters.AddWithValue("user_ids", newRecord.user_ids);
                var rc = cmd.ExecuteNonQuery();
                Console.WriteLine($"adding notified users {newRecord.user_ids} to action {newRecord.action_id} resulted in {rc}");
                return (ulong)rc;
            });
        }

        

        public static ulong Remove(uint action_id)
        {
            return DbUtils.ConnectAndExecute(conn => {
                var query =
                    "DELETE FROM engine4_advancedactivity_notificationsettings WHERE action_id=@action_id";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("action_id", action_id);

                var rc = cmd.ExecuteNonQuery();
                Console.WriteLine($"removing notified users of action {action_id} resulted in {rc}");
                return (ulong)rc;
            });
        }

        public static void Update(NotifiedUser record)
        {
            Remove((uint)record.action_id);
            Add(record);
        }
    }
}
