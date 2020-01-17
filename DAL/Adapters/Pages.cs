using System;
using System.Collections.Generic;
using System.Linq;
using KoobecaFeedController.DAL.Models;
using MySql.Data.MySqlClient;

namespace KoobecaFeedController.DAL.Adapters {
    public class Pages {
        private static Dictionary<uint, Page> _Cache;

        public static uint MaxPageNum
        {
            get
            {
                GetById(0);//to initiate the cache
                return _Cache.Keys.Max();
            }
        }

        public static Page GetById(uint pageId) {
            if (_Cache == null) {
                _Cache = new Dictionary<uint, Page>();
                GetAll().Select(p => _Cache[p.page_id] = p).ToArray();
            }
             _Cache.TryGetValue(pageId, out var page);
            if(page == null)
            {
                //try to get from DB
                page = DbUtils.ConnectAndGet(conn => {
                    var query = "SELECT page_id,title,photo_id,owner_id,follow_count FROM engine4_sitepage_pages Where page_id=@page_id";

                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("page_id", pageId);
                    var items = DbUtils.GetRecords<Page>(cmd);
                    return items;
                }).FirstOrDefault();
                _Cache[pageId] = page;
            }
            return page;
        }

        public static List<Page> GetAll() {
            return DbUtils.ConnectAndGet(conn => {
                var query = "SELECT page_id,title,photo_id,owner_id,follow_count FROM engine4_sitepage_pages";

                var cmd = new MySqlCommand(query, conn);
                var items = DbUtils.GetRecords<Page>(cmd);

                return items;
            });
        }

        public static ulong UpdateFollowCount(uint pageId,int count)
        {
            Console.WriteLine($"updating follow count of page {pageId} to {count}");
            return DbUtils.ConnectAndExecute(conn => {
                var query = "UPDATE engine4_sitepage_pages SET follow_count=@follow_count WHERE page_id = @page_id";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("page_id", pageId);
                cmd.Parameters.AddWithValue("follow_count", count);

                var rc = cmd.ExecuteNonQuery();

                return (ulong)rc;
            });
        }
    }
}