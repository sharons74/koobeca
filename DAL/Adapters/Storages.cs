using System.Collections.Generic;
using System.Linq;
using KoobecaFeedController.DAL.Models;
using MySql.Data.MySqlClient;

namespace KoobecaFeedController.DAL.Adapters {
    public class Storages {
        private static readonly Dictionary<uint, Storage> Cache = new Dictionary<uint, Storage>();

        public static Storage GetByFileId(uint fileId) {
            if (!Cache.ContainsKey(fileId)) {
                var data = DbUtils.ConnectAndGet(conn => {
                    var query = "SELECT file_id,storage_path,params FROM engine4_storage_files " +
                                "WHERE file_id=@id ";
                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("id", fileId);
                    var attachments = DbUtils.GetRecords<Storage>(cmd);
                    return attachments;
                }).FirstOrDefault();

                Cache[fileId] = data;
            }

            return Cache[fileId];
        }

        public static ulong DeleteByFileId(uint fileId)
        {
            if (Cache.ContainsKey(fileId))
            {
                Cache.Remove(fileId);
            }

            return DbUtils.ConnectAndExecute(conn => {
                var query = "DELETE FROM engine4_storage_files " +
                                "WHERE file_id=@id ";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("id", fileId);

                var rc = cmd.ExecuteNonQuery();
                return (ulong)rc;
            });
        }
    }
}