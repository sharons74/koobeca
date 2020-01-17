using KoobecaFeedController.DAL.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KoobecaFeedController.DAL.Adapters
{
    public class Reactions
    {
        //TODO cache refresh
        private static Dictionary<string, Reaction> _Cache;

        public static Reaction GetByType(string type)
        {
            if (_Cache == null)
            {
                _Cache = new Dictionary<string, Reaction>();
                GetAll().Select(r => _Cache[r.type] = r).ToArray();
            }

            _Cache.TryGetValue(type, out var reaction);
            return reaction;
        }

        public static List<Reaction> GetAll()
        {
            return DbUtils.ConnectAndGet(conn => {
                var query = "SELECT * FROM engine4_sitereaction_reactionicons";

                var cmd = new MySqlCommand(query, conn);
                var items = DbUtils.GetRecords<Reaction>(cmd);

                return items;
            });
        }
    }
}
