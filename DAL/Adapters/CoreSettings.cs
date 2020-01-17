using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace KoobecaFeedController.DAL.Adapters {
    public static class CoreSettings {
        private static Dictionary<string, string> _Cache = new Dictionary<string, string>();
        private static DateTime _CacheDate = DateTime.MinValue;
        private static readonly object Lock = new object();


        public static string Get(string name, string @default = null) {
            lock (Lock) {
                if ((DateTime.Now - _CacheDate).TotalSeconds > 60) {
                    _Cache = new Dictionary<string, string>();
                    _CacheDate = DateTime.Now;
                }

                if (!_Cache.ContainsKey(name))
                    _Cache[name] = DbUtils.ConnectAndGetSingle(conn => {
                        var query = "SELECT value FROM engine4_core_settings " +
                                    "WHERE name=@name";
                        var cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("name", name);
                        return (string) (cmd.ExecuteScalar() ?? @default);
                    });
                return _Cache[name];
            }
        }

        public static ulong Set(string name, string value, bool allowAddNew = false) {
            return DbUtils.ConnectAndExecute(conn => {
                var query = "UPDATE engine4_core_settings " +
                            "SET value=@value " +
                            "WHERE name=@name";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("name", name);
                cmd.Parameters.AddWithValue("value", value);
                var rc = cmd.ExecuteNonQuery();
                if (rc == 0 && allowAddNew) {
                    query = "INSERT INTO engine4_core_settings(name,value) " +
                            "VALUES (@name,@value)";
                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("name", name);
                    cmd.Parameters.AddWithValue("value", value);
                    rc = cmd.ExecuteNonQuery();
                }

                _Cache[name] = value;

                return (ulong)rc;
            });
        }

        public static ulong Clear(string name) {
            return DbUtils.ConnectAndExecute(conn => {
                var query = "DELETE FROM engine4_core_settings " +
                            "WHERE name=@name";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("name", name);
                var rc = cmd.ExecuteNonQuery();
                _Cache[name] = null;
                return (ulong)rc;
            });
        }

        public static Dictionary<string, string> GetAll() {
            lock (Lock) {
                return DbUtils.ConnectAndGetSingle(conn => {
                    var query = "SELECT name, value FROM engine4_core_settings";
                    var cmd = new MySqlCommand(query, conn);
                    var reader = cmd.ExecuteReader();
                    var rc = new Dictionary<string, string>();
                    while (reader.Read()) {
                        var name = reader.GetString(0);
                        var value = reader.GetString(1);
                        rc[name] = value;
                    }

                    reader.Close();
                    _Cache = rc;
                    _CacheDate = DateTime.Now;
                    return rc;
                });
            }
        }
    }
}