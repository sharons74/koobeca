using System;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace KoobecaFeedController.DAL {
    internal class DbConnection {
        private static DbConnection _instance;
        public string DatabaseName { get; set; } = string.Empty;
        public MySqlConnection Connection { get; private set; }
        private DateTime _lastConnection = DateTime.MinValue;

        public static DbConnection Instance() {
            return _instance ?? (_instance = new DbConnection());
        }
        //stam comment
        public bool Connect() {
            if ((DateTime.Now - _lastConnection).TotalMinutes > 10) Connection = null; //refresh every 10 min

            if (Connection != null) return true;
            if (string.IsNullOrEmpty(DatabaseName))
                return false;
            var userId = "root";// ConfigurationManager.AppSettings["userId"];
            var password = "Girdle0osmatism9ludefisk$";// ConfigurationManager.AppSettings["password"];
            var server = "localhost";// ConfigurationManager.AppSettings["server"];
            var connstring = $"Server={server}; database={DatabaseName}; UID={userId}; password={password} ; convert zero datetime=True";
            Console.WriteLine("creating new sql connection");
            Connection = new MySqlConnection(connstring);
            Connection.Open();
            _lastConnection = DateTime.Now;
            return true;
        }

        public void Close() {
            Connection.Close();
        }
    }
}