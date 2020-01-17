using System;
using System.Linq;
using KoobecaFeedController.DAL.Models;
using MySql.Data.MySqlClient;

namespace KoobecaFeedController.DAL.Adapters {
    public class Comments {
        public static Comment GetById(uint id) {
            return DbUtils.ConnectAndGet(conn => {
                var query = "SELECT * FROM engine4_activity_comments WHERE comment_id=@id";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("id", id);
                var comment = DbUtils.GetRecords<Comment>(cmd);
                return comment;
            }).FirstOrDefault();
        }

        public static ulong Add(Comment newComment)
        {
            return DbUtils.ConnectAndExecute(conn => {
                var query = "INSERT INTO engine4_activity_comments(resource_id,poster_type,poster_id,body,creation_date,like_count," +
                "params,parent_comment_id,attachment_type,attachment_id) " +
                                                       "VALUES (@resource_id,@poster_type,@poster_id,@body,@creation_date,@like_count," +
                "@params,@parent_comment_id,@attachment_type,@attachment_id)";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("resource_id", newComment.resource_id);
                cmd.Parameters.AddWithValue("poster_type", newComment.poster_type);
                cmd.Parameters.AddWithValue("poster_id", newComment.poster_id);
                cmd.Parameters.AddWithValue("body", newComment.body);
                cmd.Parameters.AddWithValue("creation_date", newComment.creation_date);
                cmd.Parameters.AddWithValue("like_count", newComment.like_count);
                cmd.Parameters.AddWithValue("params", newComment.@params);
                cmd.Parameters.AddWithValue("parent_comment_id", newComment.parent_comment_id);
                cmd.Parameters.AddWithValue("attachment_type", newComment.attachment_type);
                cmd.Parameters.AddWithValue("attachment_id", (uint)newComment.attachment_id);
                

                var rc = cmd.ExecuteNonQuery();

                cmd.CommandText = "Select @@Identity";
                ulong commentId = (ulong)cmd.ExecuteScalar();
                newComment.comment_id = (uint)commentId;
                return commentId;
            });
        }

        public static Comment[] GetForAction(ulong activityId)
        {
            return DbUtils.ConnectAndGet(conn => {
                var query = "SELECT * FROM engine4_activity_comments WHERE resource_id=@resource_id";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("resource_id", activityId);
                var comment = DbUtils.GetRecords<Comment>(cmd);
                return comment;
            }).ToArray();
        }

        public static Comment[] GetForComment(ulong commentId)
        {
            return DbUtils.ConnectAndGet(conn => {
                var query = "SELECT * FROM engine4_activity_comments WHERE comment_id=@comment_id OR parent_comment_id=@comment_id";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("comment_id", commentId);
                var comment = DbUtils.GetRecords<Comment>(cmd);
                return comment;
            }).ToArray();
        }

        public static ulong UpdateLikeCount(uint comment_id, uint like_count)
        {
            return DbUtils.ConnectAndExecute(conn => {
                var query = "UPDATE engine4_activity_comments SET like_count=@like_count WHERE comment_id = @comment_id";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("like_count", like_count);
                cmd.Parameters.AddWithValue("comment_id", comment_id);

                var rc = cmd.ExecuteNonQuery();

                return (ulong)rc;
            });
        }

        public static ulong UpdateParams(uint comment_id,string @params)
        {
            return DbUtils.ConnectAndExecute(conn => {
                var query = "UPDATE engine4_activity_comments SET params=@params WHERE comment_id = @comment_id";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("params", @params);
                cmd.Parameters.AddWithValue("comment_id", comment_id);
                var rc = cmd.ExecuteNonQuery();

                return (ulong)rc;
            });
        }

        public static ulong RemoveForResource(ulong actionId)
        {
            return DbUtils.ConnectAndExecute(conn => {
                var query =
                    "DELETE FROM engine4_activity_comments WHERE resource_id=@resource_id";
                Console.WriteLine(query);
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("resource_id", (uint)actionId);

                var rc = cmd.ExecuteNonQuery();
                Console.WriteLine($"effected raws {rc}");
                return (ulong)rc;
            });
        }

        public static ulong Remove(uint comment_id)
        {
            Console.WriteLine($"removing comment {comment_id} from table");
            return DbUtils.ConnectAndExecute(conn => {
                var query =
                    "DELETE FROM engine4_activity_comments WHERE comment_id=@comment_id";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("comment_id", comment_id);

                var rc = cmd.ExecuteNonQuery();
                return (ulong)rc;
            });
        }

        public static ulong Remove(uint userId, ulong actionId)
        {
            Console.WriteLine($"removing all comments from table of action {actionId} by user {userId}");
            return DbUtils.ConnectAndExecute(conn => {
                var query =
                    "DELETE FROM engine4_activity_comments WHERE resource_id=@resource_id AND poster_id=@poster_id AND poster_type='user'";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("resource_id", actionId);
                cmd.Parameters.AddWithValue("poster_id", userId);

                var rc = cmd.ExecuteNonQuery();
                return (ulong)rc;
            });
        }

        public static Comment GetByCommentId(ulong commentId)
        {
            return DbUtils.ConnectAndGet(conn => {
                var query =
                    "SELECT * FROM engine4_activity_comments WHERE comment_id=@comment_id";
                
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("comment_id",(uint)commentId);
                var items = DbUtils.GetRecords<Comment>(cmd);
                return items;
            }).FirstOrDefault();
        }
    }
}