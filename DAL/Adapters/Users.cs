using System.Collections.Generic;
using System.Linq;
using KoobecaFeedController.DAL.Models;
using MySql.Data.MySqlClient;

namespace KoobecaFeedController.DAL.Adapters {
    public class Users {
        private static Dictionary<uint, User> _Cache;

        public static User GetById(uint userId) {
            if (_Cache == null) {
                _Cache = new Dictionary<uint, User>();
                GetAll().Select(u => _Cache[u.user_id] = u).ToArray();
            }

            _Cache.TryGetValue(userId, out var user);
            if(user == null)
            {
                //TODO we need to see in all other cached DALs that we try to get to db as well
                //try to get it from db
                user = DbUtils.ConnectAndGet<User>(conn => {
                    var query = "SELECT user_id,email,displayname,photo_id FROM engine4_users where user_id=@user_id";
                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("user_id", userId);
                    return DbUtils.GetRecords<User>(cmd);
                }).FirstOrDefault();
                if(user != null)
                {
                    _Cache[userId] = user;
                }
            }
            return user;
        }

        public static List<User> GetAll() {
            return DbUtils.ConnectAndGet(conn => {
                var query = "SELECT user_id,email,displayname,photo_id FROM engine4_users";

                var cmd = new MySqlCommand(query, conn);
                var items = DbUtils.GetRecords<User>(cmd);

                return items;
            });
        }

        public static List<UserFull> GetAllFull()
        {
            return DbUtils.ConnectAndGet(conn => {
                var query = @"SELECT user_id,email,username,displayname,photo_id,status,status_date,locale,language,timezone
,show_profileviewers,level_id,invites_used,extra_invites,enabled,verified,approved,
creation_date,modified_date,lastlogin_date,update_date,member_count,view_count,comment_count,like_count,
coverphoto,coverphotoparams,view_privacy,seao_locationid,location FROM engine4_users";

                var cmd = new MySqlCommand(query, conn);
                var items = DbUtils.GetRecords<UserFull>(cmd);

                return items;
            });
        }

        public static UserFull GetFullById(int userId)
        {
            return DbUtils.ConnectAndGet<UserFull>(conn => {
                var query = @"SELECT user_id,email,username,displayname,photo_id,status,status_date,locale,language,timezone
,show_profileviewers,level_id,invites_used,extra_invites,enabled,verified,approved,
creation_date,modified_date,lastlogin_date,update_date,member_count,view_count,comment_count,like_count,
coverphoto,coverphotoparams,view_privacy,seao_locationid,location FROM engine4_users where user_id=@user_id";

                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("user_id", userId);
                return DbUtils.GetRecords<UserFull>(cmd);
            }).FirstOrDefault();
        }
    }
}