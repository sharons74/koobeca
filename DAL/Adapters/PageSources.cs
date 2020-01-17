using KoobecaFeedController.DAL.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KoobecaFeedController.DAL.Adapters
{
    public class PageSources
    {
        private static object _lock = new object();

        public static List<PageSource> GetByType(string type)
        {
            lock (_lock)
            {
                return DbUtils.ConnectAndGet(conn =>
                {
                    var query = "SELECT * FROM kfc_page_sources WHERE source_type=@type";
                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("type", type);
                    var items = DbUtils.GetRecords<PageSource>(cmd);
                    return items;
                }).ToList();
            }
        }

        public static PageSource GetBySourceId(string srcId, string type)
        {
            lock (_lock)
            {
                return DbUtils.ConnectAndGet(conn =>
                {
                    var query = "SELECT * FROM kfc_page_sources WHERE source_id=@source_id AND source_type=@type";
                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("source_id", srcId);
                    cmd.Parameters.AddWithValue("type", type);
                    var items = DbUtils.GetRecords<PageSource>(cmd);
                    return items;
                }).FirstOrDefault();
            }
        }

        public static ulong Add(ulong pageId,string source_id,string source_name,string type)
        {
            lock (_lock)
            {
                Remove(pageId, source_id, source_name, type);
                return DbUtils.ConnectAndExecute(conn =>
                {
                    var query = "INSERT INTO kfc_page_sources(page_id,source_type,source_id,source_name) " +
                                "VALUES (@page_id,@source_type,@source_id,@source_name)";
                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("page_id", pageId);
                    cmd.Parameters.AddWithValue("source_type", type);
                    cmd.Parameters.AddWithValue("source_id", source_id);
                    cmd.Parameters.AddWithValue("source_name", source_name);
                    return (ulong)cmd.ExecuteNonQuery();
                });
            }
        }

        public static ulong Remove(ulong pageId, string source_id, string source_name, string type)
        {
            lock (_lock)
            {
                if (string.IsNullOrEmpty(source_id))
                    source_id = "zksdjnhf734uejc"; //crazy string so we won't accidently delete all records with null source id

                if (string.IsNullOrEmpty(source_name))
                    source_name = "zksdjnhf734uejc"; //crazy string so we won't accidently delete all records with null source name

                return DbUtils.ConnectAndExecute(conn =>
                {
                    var query = "DELETE FROM kfc_page_sources WHERE page_id=@page_id AND source_type=@source_type AND (source_id=@source_id OR source_name=@source_name)";
                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("page_id", pageId);
                    cmd.Parameters.AddWithValue("source_type", type);
                    cmd.Parameters.AddWithValue("source_id", source_id);
                    cmd.Parameters.AddWithValue("source_name", source_name);
                    return (ulong)cmd.ExecuteNonQuery();
                });
            }
        }
    }
}
