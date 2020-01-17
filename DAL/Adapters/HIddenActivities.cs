using KoobecaFeedController.DAL.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KoobecaFeedController.DAL.Adapters
{
    public class HiddenActivities
    {
        public static ulong Add(HiddenActivity activity)
        {
            return DbUtils.ConnectAndExecute(conn => {
                var query = "INSERT INTO engine4_advancedactivity_hide(user_id,hide_resource_type,hide_resource_id) " +
                                                       "VALUES (@user_id,@hide_resource_type,@hide_resource_id)";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("user_id", activity.user_id);
                cmd.Parameters.AddWithValue("hide_resource_type", activity.hide_resource_type);
                cmd.Parameters.AddWithValue("hide_resource_id", activity.hide_resource_id);
                var rc = cmd.ExecuteNonQuery();
                return (ulong)(rc > 0 ? 1 : 0);
            });
        }

        public static ulong Remove(uint user_id, uint action_id)
        {
            return DbUtils.ConnectAndExecute(conn => {
                var query =
                    "DELETE FROM engine4_advancedactivity_hide WHERE user_id=@user_id AND hide_resource_id=@action_id";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("user_id", user_id);
                cmd.Parameters.AddWithValue("action_id", action_id);

                var rc = cmd.ExecuteNonQuery();
                return (ulong)rc;
            });
        }

        public static ulong RemoveForActivity(uint action_id)
        {
            return DbUtils.ConnectAndExecute(conn => {
                var query =
                    "DELETE FROM engine4_advancedactivity_hide WHERE hide_resource_id=@action_id";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("action_id", action_id);

                var rc = cmd.ExecuteNonQuery();
                return (ulong)rc;
            });
        }

        public static HiddenActivity Get(uint user_id, uint action_id)
        {
            return DbUtils.ConnectAndGet(conn => {
                var query = "SELECT * FROM engine4_advancedactivity_hide WHERE user_id=@user_id AND hide_resource_id=@action_id";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("user_id", user_id);
                cmd.Parameters.AddWithValue("action_id", action_id);
                var comment = DbUtils.GetRecords<HiddenActivity>(cmd);
                return comment;
            }).FirstOrDefault();
        }

        public static IEnumerable<HiddenActivity> GetForUser(ulong user_id)
        {
            return DbUtils.ConnectAndGet(conn => {
                var query = "SELECT * FROM engine4_advancedactivity_hide WHERE user_id=@user_id";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("user_id", user_id);
                var comment = DbUtils.GetRecords<HiddenActivity>(cmd);
                return comment;
            });
        }
    }
}
