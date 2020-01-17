using System;
using System.Collections.Generic;
using System.Linq;
using KoobecaFeedController.DAL.Models;
using MySql.Data.MySqlClient;

namespace KoobecaFeedController.DAL.Adapters {
    public static class UserMemberships {
        public static List<UserMembership> Get(uint id) {
            return DbUtils.ConnectAndGet(conn => {
                var query = "SELECT * FROM engine4_user_membership " +
                            "WHERE user_id=@user_id";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("user_id", id);
                var items = DbUtils.GetRecords<UserMembership>(cmd);
                return items;
            });
        }

        

        public static void AddUserFreindRequest(uint userId, uint friendId)
        {
            UserMembership um = new UserMembership();
            um.resource_id = friendId;
            um.user_id = userId;
            um.active = false;// ;
            um.resource_approved = false;  // wait for approval
            um.user_approved = true;
            AddUserMembership(um);
        }

        private static void AddUserMembership(UserMembership userMembership)
        {

             bool membershipExists = Get(userMembership.user_id).Any(u => u.resource_id == userMembership.resource_id);

             DbUtils.ConnectAndExecute(conn => {
                var query = "INSERT INTO engine4_user_membership(resource_id,user_id,active,resource_approved,user_approved,message,description) " +
                            "VALUES (@resource_id,@user_id,@active,@resource_approved,@user_approved,@message,@description)";
                 if (membershipExists)
                 {
                     query = "UPDATE engine4_user_membership set active=active,resource_approved=@resource_approved,user_approved=@user_approved,message=@message,description=@description " +
                            "where resource_id=@resource_id AND user_id=@user_id";
                 }
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("resource_id", userMembership.resource_id);
                cmd.Parameters.AddWithValue("user_id", userMembership.user_id);
                cmd.Parameters.AddWithValue("active", userMembership.active ? 1:0);
                cmd.Parameters.AddWithValue("resource_approved", userMembership.resource_approved?1:0);
                cmd.Parameters.AddWithValue("user_approved", userMembership.user_approved?1:0);
                cmd.Parameters.AddWithValue("message", userMembership.message);
                cmd.Parameters.AddWithValue("description", userMembership.description);

                var rc = cmd.ExecuteNonQuery();
                return 0;
            });
        }

        public static void RemoveFriendRequest(uint userId,uint friendId)
        {
            DbUtils.ConnectAndExecute(conn => {
                var query =
                    "DELETE FROM engine4_user_membership WHERE resource_id=@friendId AND userId=@userId" +
                    "AND poster_id=@poster_id AND poster_type='user'";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("friendId", friendId);
                cmd.Parameters.AddWithValue("userId", userId);

                var rc = cmd.ExecuteNonQuery();
                return (ulong)rc;
            });
        }
    }
}