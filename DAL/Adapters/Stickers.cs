using KoobecaFeedController.DAL.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KoobecaFeedController.DAL.Adapters
{
    public class Stickers
    {
        private static Dictionary<ulong, Sticker> _Cache = new Dictionary<ulong, Sticker>();
        

        public static Sticker GetById(uint id)
        {
            if (_Cache.ContainsKey(id))
            {
                return _Cache[id];
            }
            else
            {
                var sticker = DbUtils.ConnectAndGet(conn =>
                {
                    var query = "SELECT * FROM engine4_sitereaction_stickers where sticker_id=@id";

                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("id", id);
                    var items = DbUtils.GetRecords<Sticker>(cmd);

                    return items;
                }).FirstOrDefault();
                _Cache[id] = sticker;
                if (_Cache.Count > 1000000) _Cache = new Dictionary<ulong, Sticker>();//clean cache
                return sticker;
            }

        }
    }
}
