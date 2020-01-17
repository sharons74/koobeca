using KoobecaFeedController.DAL.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace KoobecaFeedController.DAL.Adapters
{
    public class SourceAffinities
    {
        public static ulong Add(SourceAffinity affinity)
        {
            return DbUtils.ConnectAndExecute(conn => {
                var query = "INSERT INTO kfc_source_affinity(user_id,source_id,source_type,affinity,date) " +
                                                       "VALUES (@user_id,@source_id,@source_type,@affinity,@date)";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("user_id", affinity.user_id);
                cmd.Parameters.AddWithValue("source_id", affinity.source_id);
                cmd.Parameters.AddWithValue("source_type", affinity.source_type);
                cmd.Parameters.AddWithValue("affinity", affinity.affinity);
                cmd.Parameters.AddWithValue("date", affinity.date);
                var rc = cmd.ExecuteNonQuery();

                cmd.CommandText = "Select @@Identity";
                affinity.record_id = (ulong)cmd.ExecuteScalar();
                return affinity.record_id;
            });
        }

        public static ulong UpdateAffinity(uint user_id, uint source_id, string source_type, byte affinity)
        {
            return DbUtils.ConnectAndExecute(conn => {
                var query = "UPDATE kfc_source_affinity SET affinity=@affinity WHERE user_id=@user_id AND source_type=@source_type AND source_id=@source_id";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("user_id",user_id);
                cmd.Parameters.AddWithValue("source_id",source_id);
                cmd.Parameters.AddWithValue("source_type",source_type);
                cmd.Parameters.AddWithValue("affinity",affinity);

                var rc = cmd.ExecuteNonQuery();

                return (ulong)rc;
            });
        }

        public static ulong DecreasAllLikes()
        {
            return DbUtils.ConnectAndExecute(conn => {
                var query = "UPDATE kfc_source_affinity SET affinity = affinity -1 where affinity > 1";
                var cmd = new MySqlCommand(query, conn);
                var rc = cmd.ExecuteNonQuery();
                return (ulong)rc;
            });
        }

        public static ulong RemoveAffinity(uint userId, uint sourceId, string sourceType)
        {
            return DbUtils.ConnectAndExecute(conn => {
                var query =
                    "DELETE FROM kfc_source_affinity WHERE user_id=@user_id AND source_type=@source_type AND source_id=@source_id";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("user_id", userId);
                cmd.Parameters.AddWithValue("source_type", sourceType);
                cmd.Parameters.AddWithValue("source_id", sourceId);
                var rc = cmd.ExecuteNonQuery();
                return (ulong)rc;
            });
        }

        public static IEnumerable<SourceAffinity> GetForUser(ulong user_id)
        {
            return DbUtils.ConnectAndGet(conn => {
                var query = "SELECT * FROM kfc_source_affinity WHERE user_id=@user_id";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("user_id", user_id);
                var affinity = DbUtils.GetRecords<SourceAffinity>(cmd);
                return affinity;
            });
        }
    }
}
