using KoobecaFeedController.DAL.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace KoobecaFeedController.DAL.Adapters
{
    public class Banners
    {
        public static List<Banner> GetAll()
        {
            return DbUtils.ConnectAndGet(conn => {
                var query = "SELECT file_id,highlighted,color,background_color FROM engine4_advancedactivity_banners";

                var cmd = new MySqlCommand(query, conn);
                var items = DbUtils.GetRecords<Banner>(cmd);

                return items;
            });
        }
    }
}
