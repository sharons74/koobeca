using System;
using System.Collections.Generic;
using System.Linq;
using KoobecaFeedController.DAL.Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace KoobecaFeedController.DAL.Adapters {
    public static class ActivityAttachments {
        public static List<ActivityAttachment> Get(params uint[] actionIds) {
            return DbUtils.ConnectAndGet(conn => {
                var query = "SELECT * FROM engine4_activity_attachments " +
                            "WHERE FIND_IN_SET(action_id, @ids) " +
                            "ORDER BY action_id";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("ids", string.Join(",", actionIds));
                var attachments = DbUtils.GetRecords<ActivityAttachment>(cmd);
                return attachments;
            });
        }

        public static ulong Add(ActivityAttachment newAttachment) {
            Console.WriteLine($"adding attachment to table:{JsonConvert.SerializeObject(newAttachment)}");
            return DbUtils.ConnectAndExecute(conn => {
                var query = "INSERT INTO engine4_activity_attachments(action_id,id,type,mode) " +
                            "VALUES (@action_id,@id,@type,@mode)";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("action_id", newAttachment.action_id);
                cmd.Parameters.AddWithValue("id", newAttachment.id);
                cmd.Parameters.AddWithValue("type", newAttachment.type);
                cmd.Parameters.AddWithValue("mode", newAttachment.mode);
                var rc = cmd.ExecuteNonQuery();
                cmd.CommandText = "Select @@Identity";
                var attachmentId = (ulong)cmd.ExecuteScalar();
                return attachmentId;
            });
        }

        public static ulong DeleteForActivity(ulong actionId) {
            Console.WriteLine($"deleting attachment for activity {actionId} from table");
            return DbUtils.ConnectAndExecute(conn => {
                var query = "DELETE FROM engine4_activity_attachments " +
                            "WHERE action_id=@action_id";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("action_id", actionId);
                var rc = (uint)cmd.ExecuteNonQuery();
                return rc;
            });
        }

        public static List<ActivityAttachment> GetByTypeAndId(params (string, uint)[] objectIds)
        {
            foreach(var pair in objectIds)
            {
                Console.WriteLine($"getteing attachment {pair.Item1} {pair.Item2}");
            }
            return DbUtils.ConnectAndGet(conn =>
            {
                var query = "SELECT * FROM engine4_activity_attachments " +
                            "WHERE FIND_IN_SET(concat(\"@\",type,\"$\",id,\"@\"), @ids) " +
                            "ORDER BY action_id";
                var cmd = new MySqlCommand(query, conn);
                var keys = objectIds.Select(x => $"@{x.Item1}${x.Item2}@");
                cmd.Parameters.AddWithValue("ids", string.Join(",", keys));
                var attachments = DbUtils.GetRecords<ActivityAttachment>(cmd);
                return attachments;
            });
        }


        public static List<ActivityAttachment> GetByObjectId(uint objectId) {
            return DbUtils.ConnectAndGet(conn => {
                var query = "SELECT * FROM engine4_activity_attachments " +
                            "WHERE id=@id ";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("id", objectId);
                var attachments = DbUtils.GetRecords<ActivityAttachment>(cmd);
                return attachments;
            });
        }
    }
}