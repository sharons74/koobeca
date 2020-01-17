using System;
using System.Collections.Generic;
using System.Linq;
using KoobecaFeedController.DAL.Models;
using MySql.Data.MySqlClient;

namespace KoobecaFeedController.DAL.Adapters {
    public class Videos {
        private static Dictionary<ulong, Video> _Cache = new Dictionary<ulong, Video>();

        public static Video GetById(uint id) {

            if (_Cache.ContainsKey(id))
            {
                return _Cache[id];
            }
            else
            {
                var video = DbUtils.ConnectAndGet(conn =>
                {
                    var query =
                        "SELECT video_id,title,description,photo_id,file_id,code,type,comment_count,like_count,owner_id,owner_type FROM engine4_video_videos where video_id=@id";

                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("id", id);
                    var items = DbUtils.GetRecords<Video>(cmd);

                    return items;
                }).FirstOrDefault();
                _Cache[id] = video;
                if (_Cache.Count > 1000000) _Cache = new Dictionary<ulong, Video>(); //reset cache
                return video;
            }
        }

        public static ulong Add(Video newVideo)
        {
            //sanity check on video title , make sure it is no more than 100 chars
            if(newVideo.title != null && newVideo.title.Length > 100)
            {
                newVideo.title = newVideo.title.Substring(0, 97) + "...";
            }

            return DbUtils.ConnectAndExecute(conn => {
                var query = "INSERT INTO engine4_video_videos(code,description,title,type,owner_id,owner_type,creation_date,modified_date," +
                "view_count,comment_count,rating,category_id,status,file_id,duration,rotation,featured,favourite_count,sponsored," +
                "seao_locationid,location) " +
                            "VALUES (@code,@description,@title,@type,@owner_id,@owner_type,@date,@date,0,0,0,0,0,0,0,0,0,0,0,0,'')";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("code", newVideo.code);
                cmd.Parameters.AddWithValue("description", newVideo.description);
                cmd.Parameters.AddWithValue("title", newVideo.title);
                cmd.Parameters.AddWithValue("type", newVideo.type);
                cmd.Parameters.AddWithValue("owner_id", newVideo.owner_id);
                cmd.Parameters.AddWithValue("owner_type", newVideo.owner_type);
                cmd.Parameters.AddWithValue("date", DateTime.Now);

                var rc = cmd.ExecuteNonQuery();

                cmd.CommandText = "Select @@Identity";
                newVideo.video_id = (uint)(ulong)cmd.ExecuteScalar();
                if (newVideo.video_id > 0)
                {
                    _Cache[newVideo.video_id] = newVideo;
                }
                return newVideo.video_id;
            });
        }
    }
}