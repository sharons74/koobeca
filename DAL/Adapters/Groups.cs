using System;
using System.Collections.Generic;
using System.Linq;
using KoobecaFeedController.DAL.Models;
using MySql.Data.MySqlClient;

namespace KoobecaFeedController.DAL.Adapters {
    public class Groups {
        private static Dictionary<uint, Group> _Cache;

        public static Group GetById(uint groupId) {
            if (_Cache == null) {
                _Cache = new Dictionary<uint, Group>();
                GetAll().Select(g => _Cache[g.group_id] = g).ToArray();
            }
            _Cache.TryGetValue(groupId, out var group);
            if (group == null)
            {
                //try to get from DB
                group = DbUtils.ConnectAndGet(conn => {
                    var query = "SELECT group_id,title,photo_id,owner_id,follow_count FROM engine4_sitegroup_groups Where group_id=@group_id";

                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("group_id", groupId);
                    var items = DbUtils.GetRecords<Group>(cmd);
                    return items;
                }).FirstOrDefault();
                _Cache[groupId] = group;
            }
            return group;
        }

        public static List<Group> GetAll() {
            return DbUtils.ConnectAndGet(conn => {
                var query = "SELECT group_id,title,photo_id,owner_id,follow_count FROM engine4_sitegroup_groups";

                var cmd = new MySqlCommand(query, conn);
                var items = DbUtils.GetRecords<Group>(cmd);

                return items;
            });
        }

        public static List<int>  GetUserGroups(int userId)
        {  
            return DbUtils.ConnectAndGet(conn => {
                var query = "select group_id from engine4_sitegroup_membership where user_id = @user_id";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("user_id", userId);
                var items = DbUtils.GetRecords<GroupId>(cmd).Select(g=>g.group_id).ToList();
                return items;
            });
        }

        public static ulong UpdateFollowCount(uint groupId, int count)
        {
            Console.WriteLine($"updating follow count of group {groupId} to {count}");
            return DbUtils.ConnectAndExecute(conn => {
                var query = "UPDATE engine4_sitegroup_groups SET follow_count=@follow_count WHERE group_id = @group_id";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("group_id", groupId);
                cmd.Parameters.AddWithValue("follow_count", count);

                var rc = cmd.ExecuteNonQuery();
                return (ulong)rc;
            });
        }
    }
    
}