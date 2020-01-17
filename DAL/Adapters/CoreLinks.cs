using System;
using System.Collections.Generic;
using System.Linq;
using KoobecaFeedController.DAL.Models;
using MySql.Data.MySqlClient;

namespace KoobecaFeedController.DAL.Adapters
{
    public class CoreLinks
    {
        private static Dictionary<uint, CoreLink> _Cache = new Dictionary<uint, CoreLink>();

        public static CoreLink GetById(uint id)
        {
            if (!_Cache.ContainsKey(id))
            {
                var corelink = DbUtils.ConnectAndGet(conn =>
                {
                    var query = "SELECT * FROM engine4_core_links WHERE link_id=@id";
                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("id", id);
                    var link = DbUtils.GetRecords<CoreLink>(cmd);
                    return link;
                }).FirstOrDefault();

                _Cache[id] = corelink;
                if (_Cache.Count > 1000000) _Cache = new Dictionary<uint, CoreLink>(); //reset cache
                return corelink;
            }
            else
            {
                return _Cache[id];
            }
        }

        public static ulong UpdateParent(uint linkId, ulong parentId, string parentType)
        {
            return DbUtils.ConnectAndExecute(conn => {
                var query = "UPDATE engine4_core_links SET parent_id=@parent_id,parent_type=@parent_type WHERE link_id=@link_id";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("parent_id", parentId);
                cmd.Parameters.AddWithValue("parent_type", parentType);
                cmd.Parameters.AddWithValue("link_id", linkId);

                var rc = cmd.ExecuteNonQuery();

                if(rc > 0)
                {
                    //update the cache
                   if(_Cache.TryGetValue(linkId,out CoreLink link) && link != null)
                    {
                        link.parent_type = parentType;
                        link.parent_id = (uint)parentId;
                    }
                }

                return (ulong)rc;
            });
        }
    }
}