using System;
using System.Collections.Generic;
using System.Linq;
using KoobecaFeedController.DAL.Models;
using MySql.Data.MySqlClient;

namespace KoobecaFeedController.DAL.Adapters {
    public class AlbumPhotos {
        private static Dictionary<ulong,AlbumPhoto> _Cache = new Dictionary<ulong, AlbumPhoto>();

        public static List<AlbumPhoto> GetAll() {
            return DbUtils.ConnectAndGet(conn => {
                var query = "SELECT * FROM engine4_album_photos";

                var cmd = new MySqlCommand(query, conn);
                var items = DbUtils.GetRecords<AlbumPhoto>(cmd);

                return items;
            });
        }

        public static AlbumPhoto GetById(uint id) {

            if (_Cache.ContainsKey(id))
            {
                return _Cache[id];
            }
            else
            {
                var photo = DbUtils.ConnectAndGet(conn =>
                {
                    var query = "SELECT * FROM engine4_album_photos where photo_id=@id";

                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("id", id);
                    var items = DbUtils.GetRecords<AlbumPhoto>(cmd);

                    return items;
                }).FirstOrDefault();
                _Cache[id] = photo;
                if (_Cache.Count > 1000000) _Cache = new Dictionary<ulong, AlbumPhoto>();//clean cache
                return photo;
            }
            
        }
    }
}