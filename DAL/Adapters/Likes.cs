using System;
using System.Collections.Generic;
using System.Linq;
using KoobecaFeedController.DAL.Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace KoobecaFeedController.DAL.Adapters {
    public class Likes {
        public static IEnumerable<Like> GetByResourceId(uint resourceId) {
            return DbUtils.ConnectAndGet(conn => {
                var query = "SELECT * FROM engine4_activity_likes WHERE resource_id=@resourceId";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("resourceId", resourceId);
                var items = DbUtils.GetRecords<Like>(cmd);
                return items;
            });
        }

        public static Like GetByResourceAndPoster(uint resourceId, string resourceType, uint posterId,
            string posterType) {
            return DbUtils.ConnectAndGet(conn => {
                var query =
                    "SELECT like_id,resource_id,poster_type,poster_id,reaction FROM engine4_core_likes WHERE resource_id=@resourceId AND resource_type=@resourceType AND " +
                    "poster_id=@posterId AND poster_type=@posterType";
                if (resourceType == "activity_action")
                    query = "SELECT * FROM engine4_activity_likes WHERE resource_id=@resourceId AND " +
                            "poster_id=@posterId AND poster_type=@posterType";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("resourceId", resourceId);
                cmd.Parameters.AddWithValue("resourceType", resourceType);
                cmd.Parameters.AddWithValue("posterId", posterId);
                cmd.Parameters.AddWithValue("posterType", posterType);
                var items = DbUtils.GetRecords<Like>(cmd);
                return items;
            }).FirstOrDefault();
        }

        public static ulong RemoveForResource(ulong actionId)
        {
            return DbUtils.ConnectAndExecute(conn => {
                var query =
                    "DELETE FROM engine4_activity_likes WHERE resource_id=@resource_id";
                Console.WriteLine(query);
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("resource_id", actionId);

                var rc = cmd.ExecuteNonQuery();
                Console.WriteLine($"effected raws {rc}");
                return (ulong)rc;
            });
        }

        public static ulong UpdateReaction(uint poster_id, ulong resource_id, string reaction)
        {
            Console.WriteLine($"updating reaction in likes to {reaction}");
            return DbUtils.ConnectAndExecute(conn => {
                var query = "UPDATE engine4_activity_likes SET reaction=@reaction WHERE resource_id = @resource_id AND poster_id = @poster_id";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("resource_id", resource_id);
                cmd.Parameters.AddWithValue("poster_id", poster_id);
                cmd.Parameters.AddWithValue("reaction", reaction);
                
                var rc = cmd.ExecuteNonQuery();

                return (ulong)rc;
            });
        }

        public static ulong Add(Like newLike) {
            Console.WriteLine($"adding like to table:{JsonConvert.SerializeObject(newLike)}");
            return DbUtils.ConnectAndExecute(conn => {
                var query = "INSERT INTO engine4_activity_likes(resource_id,poster_type,poster_id,reaction) " +
                            "VALUES (@resource_id,@poster_type,@poster_id,@reaction)";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("resource_id", newLike.resource_id);
                cmd.Parameters.AddWithValue("poster_type", newLike.poster_type);
                cmd.Parameters.AddWithValue("poster_id", newLike.poster_id);
                cmd.Parameters.AddWithValue("reaction", newLike.reaction);

                var rc = cmd.ExecuteNonQuery();

                cmd.CommandText = "Select @@Identity";
                return (ulong)cmd.ExecuteScalar();
                //return rc;
            });
        }

        public static ulong Remove(uint userId, ulong actionId) {
            Console.WriteLine($"removing like from table of action {actionId} by user {userId}");
            return DbUtils.ConnectAndExecute(conn => {
                var query =
                    "DELETE FROM engine4_activity_likes WHERE resource_id=@resource_id AND poster_id=@poster_id";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("resource_id", actionId);
                cmd.Parameters.AddWithValue("poster_id", userId);

                var rc = cmd.ExecuteNonQuery();
                return (ulong)rc;
            });
        }
    }
}