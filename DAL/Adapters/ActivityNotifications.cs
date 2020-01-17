using KoobecaFeedController.DAL.Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace KoobecaFeedController.DAL.Adapters
{
    public class ActivityNotifications
    {
        public static ulong Add(ActivityNotification newActivityNotification)
        {
            Console.WriteLine($"adding notification to table:{JsonConvert.SerializeObject(newActivityNotification)}");
            return DbUtils.ConnectAndExecute(conn => {
                var query = "INSERT INTO engine4_activity_notifications(user_id,subject_type,subject_id,object_type,object_id,type,params,mitigated,date) " +
                                                        "VALUES (@user_id,@subject_type,@subject_id,@object_type,@object_id,@type,@params,@mitigated,@date)";

                //var query = "INSERT INTO engine4_activity_notifications VALUES (@user_id,@subject_type,@subject_id,@object_type,@object_id,@type,@params,@read,@mitigated,@date,@show)";

                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("user_id", newActivityNotification.user_id);
                cmd.Parameters.AddWithValue("subject_type", newActivityNotification.subject_type);
                cmd.Parameters.AddWithValue("subject_id", newActivityNotification.subject_id);
                cmd.Parameters.AddWithValue("object_type", newActivityNotification.object_type);
                cmd.Parameters.AddWithValue("object_id", newActivityNotification.object_id);
                cmd.Parameters.AddWithValue("type", newActivityNotification.type);
                cmd.Parameters.AddWithValue("params", newActivityNotification.@params);
                cmd.Parameters.AddWithValue("read", newActivityNotification.read);
                cmd.Parameters.AddWithValue("mitigated", newActivityNotification.mitigated);
                cmd.Parameters.AddWithValue("date", newActivityNotification.date);
                cmd.Parameters.AddWithValue("show", newActivityNotification.show);
                cmd.ExecuteNonQuery();

                cmd.CommandText = "Select @@Identity";
                ulong nid = (ulong)cmd.ExecuteScalar();
                newActivityNotification.notification_id = (uint)nid;
                return (ulong)newActivityNotification.notification_id;
            });
        }

        public static ulong RemoveForActivity(uint action_id)
        {
            return DbUtils.ConnectAndExecute(conn => {
                var query =
                    "DELETE FROM engine4_activity_notifications WHERE object_type='activity_action' AND  object_id =@object_id";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("object_id", action_id);

                var rc = cmd.ExecuteNonQuery();
                return (ulong)rc;
            });
        }

        public static IEnumerable<ActivityNotification> GetByUserId(uint userId)
        {
            return DbUtils.ConnectAndGet(conn => {
                var query =
                    "SELECT * FROM engine4_activity_notifications WHERE user_id=@user_id limit 20";

                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("user_id",userId);
                var items = DbUtils.GetRecords<ActivityNotification>(cmd);

                return items;
            });
        }
    }
}
