using System;
using System.Collections.Generic;
using KoobecaFeedController.DAL.Models;
using MySql.Data.MySqlClient;

namespace KoobecaFeedController.DAL.Adapters {
    public class WordStylings {
        private static List<WordStyling> _Cache;
        private static DateTime _CacheDate = DateTime.MinValue;
        private static readonly object Lock = new object();

        public static List<WordStyling> GetAll() {
            lock (Lock) {
                if ((DateTime.Now - _CacheDate).TotalSeconds > 60) {
                    _Cache = DbUtils.ConnectAndGet(conn => {
                        var query = "SELECT * FROM engine4_advancedactivity_words";

                        var cmd = new MySqlCommand(query, conn);
                        var items = DbUtils.GetRecords<WordStyling>(cmd);

                        return items;
                    });
                    _CacheDate = DateTime.Now;
                }
            }

            return _Cache;
        }
    }
}