using System;
using System.Collections.Generic;
using System.Linq;
using KoobecaFeedController.DAL.Models;
using MySql.Data.MySqlClient;

namespace KoobecaFeedController.DAL.Adapters {
    public class ActionTypes {
        private static readonly object Lock = new object();
        private static Dictionary<string, ActionType> _Map;

        public static List<ActionType> GetAll() {
            return DbUtils.ConnectAndGet(conn => {
                var query = "SELECT * FROM engine4_activity_actiontypes";

                var cmd = new MySqlCommand(query, conn);
                var items = DbUtils.GetRecords<ActionType>(cmd);

                return items;
            });
        }

        public static ActionType Get(string type) {
            if (_Map == null) {
                _Map = new Dictionary<string, ActionType>();
                GetAll().Select(a => _Map[a.type] = a).ToList();
                FixMap();
            }

            _Map.TryGetValue(type, out ActionType res);
            return res;
        }

        private static void FixMap()
        {
            _Map["share"].body = _Map["share"].body.Replace("var:", "item:$object:");
        }
    }
}