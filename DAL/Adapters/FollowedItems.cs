using KoobecaFeedController.DAL.Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KoobecaFeedController.DAL.Adapters
{
    public class FollowedItems
    {
        public static List<FollowedItem> Get(uint userId)
        {
            return DbUtils.ConnectAndGet(conn => {
                var query = "SELECT * FROM engine4_seaocore_follows " +
                            "WHERE poster_id=@poster_id AND poster_type='user'";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("poster_id", userId);
                var items = DbUtils.GetRecords<FollowedItem>(cmd);
                return items;
            });
        }

        public static FollowedItem GetFollow(uint userId,ulong srcId, string srcType)
        {
            return DbUtils.ConnectAndGet(conn => {
                var query =
                    "SELECT * FROM engine4_seaocore_follows WHERE resource_id=@resource_id AND resource_type=@resource_type " +
                    "AND poster_id=@poster_id AND poster_type='user'";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("resource_id", srcId);
                cmd.Parameters.AddWithValue("resource_type", srcType);
                cmd.Parameters.AddWithValue("poster_id", userId);
                return DbUtils.GetRecords<FollowedItem>(cmd);
            }).FirstOrDefault();
        }

        public static ulong Add(FollowedItem newFollow)
        {
            Console.WriteLine($"adding follow to table:{JsonConvert.SerializeObject(newFollow)}");
            return DbUtils.ConnectAndExecute(conn => {
                var query = "INSERT INTO engine4_seaocore_follows(resource_type,resource_id,poster_type,poster_id,creation_date) " +
                            "VALUES (@resource_type,@resource_id,@poster_type,@poster_id,@creation_date)";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("resource_type", newFollow.resource_type);
                cmd.Parameters.AddWithValue("resource_id", newFollow.resource_id);
                cmd.Parameters.AddWithValue("poster_type", newFollow.poster_type);
                cmd.Parameters.AddWithValue("poster_id", newFollow.poster_id);
                cmd.Parameters.AddWithValue("creation_date", newFollow.creation_date);

                var rc = cmd.ExecuteNonQuery();

                cmd.CommandText = "Select @@Identity";
                newFollow.follow_id = (uint)(ulong)cmd.ExecuteScalar();
                return newFollow.follow_id;
            });
        }

        public static ulong RemoveFollow(ulong userId, ulong srcId,string srcType)
        {
            Console.WriteLine($"removing folow of page {srcId} from table by user {userId}");
            return DbUtils.ConnectAndExecute(conn => {
                var query =
                    "DELETE FROM engine4_seaocore_follows WHERE resource_id=@resource_id AND resource_type=@resource_type " +
                    "AND poster_id=@poster_id AND poster_type='user'";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("resource_id", srcId);
                cmd.Parameters.AddWithValue("resource_type", srcType);
                cmd.Parameters.AddWithValue("poster_id", userId);

                var rc = cmd.ExecuteNonQuery();
                return (ulong)rc;
            });
        }
    }
}
