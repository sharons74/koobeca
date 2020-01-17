using KoobecaFeedController.DAL.Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KoobecaFeedController.DAL.Adapters
{
    public class CoreLikes
    {
        public static ulong Add(CoreLike newLike)
        {
            Console.WriteLine($"adding like to table:{JsonConvert.SerializeObject(newLike)}");
            return DbUtils.ConnectAndExecute(conn => {
                var query = "INSERT INTO engine4_core_likes(resource_type,resource_id,poster_type,poster_id,reaction,creation_date) " +
                            "VALUES (@resource_type,@resource_id,@poster_type,@poster_id,@reaction,@creation_date)";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("resource_type", newLike.resource_type);
                cmd.Parameters.AddWithValue("resource_id", newLike.resource_id);
                cmd.Parameters.AddWithValue("poster_type", newLike.poster_type);
                cmd.Parameters.AddWithValue("poster_id", newLike.poster_id);
                cmd.Parameters.AddWithValue("reaction", newLike.reaction);
                cmd.Parameters.AddWithValue("creation_date", newLike.creation_date);

                var rc = cmd.ExecuteNonQuery();

                cmd.CommandText = "Select @@Identity";
                newLike.like_id = (uint)(ulong)cmd.ExecuteScalar();
                return newLike.like_id;
            });
        }

        public static ulong RemoveCommentLike(ulong comment_id, uint userId)
        {
            Console.WriteLine($"removing like from table of comment {comment_id} by user {userId}");
            return DbUtils.ConnectAndExecute(conn => {
                var query =
                    "DELETE FROM engine4_core_likes WHERE resource_id=@comment_id AND resource_type='activity_comment' " +
                    "AND poster_id=@poster_id AND poster_type='user'";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("comment_id", comment_id);
                cmd.Parameters.AddWithValue("poster_id", userId);

                var rc = cmd.ExecuteNonQuery();
                return (ulong)rc;
            });
        }

        public static IEnumerable<CoreLike> CountByResource(uint resourceId,string resourceType)
        {
            return DbUtils.ConnectAndGet(conn => {
                var query = "SELECT * FROM engine4_core_likes WHERE resource_id=@resourceId AND resource_type=@resourceType";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("resourceId", resourceId);
                cmd.Parameters.AddWithValue("resourceType", resourceType);
                var items = DbUtils.GetRecords<CoreLike>(cmd);
                return items;
            });
        }
    }
}
