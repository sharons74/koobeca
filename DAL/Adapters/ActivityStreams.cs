using KoobecaFeedController.DAL.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace KoobecaFeedController.DAL.Adapters
{
    public class ActivityStreams
    {
        public static ulong Add(ActivityStream newActivityStream)
        {
            return DbUtils.ConnectAndExecute(conn => {
                var query = "INSERT INTO engine4_activity_streams(target_type,target_id,subject_type,subject_id,object_type,object_id,type,action_id) " +
                            "VALUES (@target_type,@target_id,@subject_type,@subject_id,@object_type,@object_id,@type,@action_id)";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("target_type", newActivityStream.target_type);
                cmd.Parameters.AddWithValue("target_id", newActivityStream.target_id);
                cmd.Parameters.AddWithValue("subject_type", newActivityStream.subject_type);
                cmd.Parameters.AddWithValue("subject_id", newActivityStream.subject_id);
                cmd.Parameters.AddWithValue("object_type", newActivityStream.object_type);
                cmd.Parameters.AddWithValue("object_id", newActivityStream.object_id);
                cmd.Parameters.AddWithValue("type", newActivityStream.type);
                cmd.Parameters.AddWithValue("action_id", newActivityStream.action_id);

                cmd.ExecuteNonQuery();
                return 0;
            });
        }
    }
}
