using KoobecaFeedController.DAL.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KoobecaFeedController.DAL.Adapters
{
    public class SavedActivities
    {

        public static ulong Add(SavedActivity activity)
        {
            return DbUtils.ConnectAndExecute(conn => {
                var query = "INSERT INTO engine4_advancedactivity_savefeeds(user_id,action_type,action_id) " +
                                                       "VALUES (@user_id,@action_type,@action_id)";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("user_id", activity.user_id);
                cmd.Parameters.AddWithValue("action_type", activity.action_type);
                cmd.Parameters.AddWithValue("action_id", activity.action_id);
                var rc = cmd.ExecuteNonQuery();
                return (ulong)(rc > 0? 1:0);
            });
        }

        public static ulong Remove(int user_id,int action_id)
        {
            return DbUtils.ConnectAndExecute(conn => {
                var query =
                    "DELETE FROM engine4_advancedactivity_savefeeds WHERE user_id=@user_id AND action_id=@action_id";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("user_id",user_id);
                cmd.Parameters.AddWithValue("action_id", action_id);

                var rc = cmd.ExecuteNonQuery();
                return (ulong)rc;
            });
        }

        public static SavedActivity Get(int user_id, int action_id)
        {
            return DbUtils.ConnectAndGet(conn => {
                var query = "SELECT * FROM engine4_advancedactivity_savefeeds WHERE user_id=@user_id AND action_id=@action_id";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("user_id", user_id);
                cmd.Parameters.AddWithValue("action_id", action_id);
                var comment = DbUtils.GetRecords<SavedActivity>(cmd);
                return comment;
            }).FirstOrDefault();
        }

        public static IEnumerable<SavedActivity> GetForUser(int user_id)
        {
            return DbUtils.ConnectAndGet(conn => {
                var query = "SELECT * FROM engine4_advancedactivity_savefeeds WHERE user_id=@user_id";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("user_id", user_id);
                var comment = DbUtils.GetRecords<SavedActivity>(cmd);
                return comment;
            });
        }
    }
}
