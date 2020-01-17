using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using MySql.Data.MySqlClient;

namespace KoobecaFeedController.DAL {
    internal static class DbUtils {
        private static readonly Dictionary<Type, List<(PropertyInfo, int)>> Cache =
            new Dictionary<Type, List<(PropertyInfo, int)>>();

        public static List<T> GetRecords<T>(MySqlCommand cmd) where T : new() {
            var rc = new List<T>();

            var reader = cmd.ExecuteReader();
            using (reader) {
                var gtype = typeof(T);
                var columns = GetColumns(gtype, reader);
                while (reader.Read()) {
                    var row = new T();
                    SetValues(columns, reader, row);
                    rc.Add(row);
                }
            }

            //reader.Close();
            return rc;
        }

        private static List<(PropertyInfo property, int ordinal)> GetColumns(Type gtype, IDataRecord reader) {
            if (!Cache.ContainsKey(gtype)) {
                var columns = (from p in
                            gtype.GetProperties(BindingFlags.Public |
                                                BindingFlags.Instance)
                        let attr = p.GetCustomAttributes(typeof(MySqlIgnoreAttribute), true)
                        where attr.Length == 0
                        select (property: p, ordinal: reader.GetOrdinal(p.Name))
                    ).ToList();
                Cache[gtype] = columns;
            }

            return Cache[gtype];
        }

        private static void SetValues(IReadOnlyCollection<(PropertyInfo property, int ordinal)> columns,
            IDataRecord reader, object row) {
            var vals = new object[columns.Count];
            reader.GetValues(vals);
            foreach (var col in columns) {
                var val = vals[col.ordinal];
                if (val is DateTime d)
                    col.property.SetValue(row, d.Spec());
                else
                    col.property.SetValue(row, val is DBNull ? null : val);
            }
        }

        public static List<T> ConnectAndGet<T>(Func<MySqlConnection, List<T>> action) where T : new() {
            var rc = new List<T>();

            var dbCon = DbConnection.Instance();
            dbCon.DatabaseName = "koobdb";
            if (dbCon.Connect()) return action(dbCon.Connection);
            dbCon.Close();
            return rc;
        }

        public static T ConnectAndGetSingle<T>(Func<MySqlConnection, T> action) where T : class {
            var dbCon = DbConnection.Instance();
            dbCon.DatabaseName = "koobdb";
            if (dbCon.Connect()) return action(dbCon.Connection);
            dbCon.Close();
            return null;
        }

        public static ulong ConnectAndExecute(Func<MySqlConnection, ulong> action) {
            var dbCon = DbConnection.Instance();
            dbCon.DatabaseName = "koobdb";
            if (dbCon.Connect()) return action(dbCon.Connection);
            dbCon.Close();
            return 0;
        }
    }
}