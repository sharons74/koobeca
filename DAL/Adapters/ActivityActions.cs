using System;
using System.Collections.Generic;
using System.Linq;
using KoobecaFeedController.DAL.Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace KoobecaFeedController.DAL.Adapters {
    public static class ActivityActions {
        public static List<ActivityAction> Get(uint id, int limit,
            bool getAttachments = false,
            bool getRelatedActions = false) {
            return DbUtils.ConnectAndGet(conn => {
                var query = "SELECT * FROM engine4_activity_actions " +
                            "WHERE subject_type='user' AND subject_id=@subject_id " +
                            "ORDER BY action_id DESC " +
                            "LIMIT @limit";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("subject_id", id);
                cmd.Parameters.AddWithValue("limit", limit);
                var items = DbUtils.GetRecords<ActivityAction>(cmd);

                if (getAttachments) {
                    // We have the items records, now fetch all attachments belonging to any
                    // of the items and nest them inside each item's object

                    var ids = items.Select(x => x.action_id).ToArray();
                    var attachments = ActivityAttachments.Get(ids).ToLookup(x => x.action_id);
                    foreach (var item in items)
                        item.Attachments = !attachments.Contains(item.action_id)
                            ? new List<ActivityAttachment>()
                            : attachments[item.action_id].ToList();
                }

                if (getRelatedActions) {
                    // Extract all (object_type,object_id) of all items, but excludes those which
                    // are equal to the (subject_type,subject_id) of their record as these are 'self'
                    // actions.
                    var objectIds = items
                        .Where(x => x.subject_type != x.object_type || x.subject_id != x.object_id)
                        .Select(x => (x.object_type, x.object_id))
                        .ToArray();

                    // Collect all related actions by two means:
                    //  1. Go over attachments by (object_type,object_id) and collect their action ids
                    //  2. Go over actions by (object_type,object_id) and collect their action ids
                    var rel = GetRelatedActionsInternal(objectIds);

                    foreach (var item in items) {
                        var key = (item.object_type, item.object_id);
                        item.RelatedActionIds = rel.Contains(key)
                            ? rel[key].Except(new[] {item.action_id}).ToList()
                            : new List<uint>();

                        // Special case where an action relates to another action directly.
                        // In this case add the other action manually to the list of related actions.
                        if (item.object_type == "activity_action") item.RelatedActionIds.Add(item.object_id);
                    }
                }

                return items;
            });
        }

        public static ulong RemoveRefActivities(ulong activityId)
        {
            return DbUtils.ConnectAndExecute(conn => {
                var query =
                    "DELETE FROM engine4_activity_actions WHERE object_id=@object_id AND object_type='activity_action'";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("object_id", activityId);
                
                var rc = cmd.ExecuteNonQuery();
                Console.WriteLine($"effectedm raws {rc}");
                return (ulong)rc;
            });
        }



        //public static ILookup<(string, uint), uint> GetRelatedActions(params uint[] actionIds) {
        //    var actions = GetByID(actionIds);
        //    var objectIds = actions
        //        .Where(x => x.subject_type != x.object_type || x.subject_id != x.object_id)
        //        .Select(x => (x.object_type, x.object_id))
        //        .ToArray();
        //    return GetRelatedActionsInternal(objectIds);
        //}

        private static ILookup<(string, uint), uint> GetRelatedActionsInternal(params (string, uint)[] objectIds) {
            var attachments = ActivityAttachments.GetByTypeAndId(objectIds);
            var lst = attachments.Select(x => ((x.type, x.id), x.action_id));
            var directRels = GetByObjectTypeAndIdInternal(objectIds);
            var map = lst.Union(directRels).ToLookup(x => x.Item1, x => x.Item2);

            return map;
        }

        private static List<((string, uint), uint)> GetByObjectTypeAndIdInternal(params (string, uint)[] objectIds) {
            return DbUtils.ConnectAndGet(conn => {
                var query = "SELECT action_id, object_type, object_id FROM engine4_activity_actions " +
                            "WHERE FIND_IN_SET(concat(\"@\",object_type,\"$\",object_id,\"@\"), @ids) " +
                            "ORDER BY action_id";
                var cmd = new MySqlCommand(query, conn);
                var keys = objectIds.Select(x => $"@{x.Item1}${x.Item2}@");
                cmd.Parameters.AddWithValue("ids", string.Join(",", keys));
                var rc = new List<((string, uint), uint)>();
                var reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    var actionId = reader.GetUInt32(0);
                    var objectType = reader.GetString(1);
                    var objectId = reader.GetUInt32(2);
                    rc.Add(((objectType, objectId), actionId));
                }

                reader.Close();
                return rc;
            });
        }

        public static ActivityAction GetById(uint actionId, bool getAttachments = true) {
            return DbUtils.ConnectAndGet(conn => {
                var query = "SELECT * FROM engine4_activity_actions WHERE FIND_IN_SET(action_id, @ids)";

                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("ids", string.Join(",", actionId));
                var items = DbUtils.GetRecords<ActivityAction>(cmd);

                if (getAttachments) {
                    // We have the items records, now fetch all attachments belonging to any
                    // of the items and nest them inside each item's object

                    var ids = items.Select(x => x.action_id).ToArray();
                    var attachments = ActivityAttachments.Get(ids).ToLookup(x => x.action_id);
                    foreach (var item in items)
                        item.Attachments = !attachments.Contains(item.action_id)
                            ? new List<ActivityAttachment>()
                            : attachments[item.action_id].ToList();
                }

                 return items;
            }).FirstOrDefault();
        }

        public static IEnumerable<ActivityAction> GetByObjectId(uint objectId, string objectType, string type = null) {
            return DbUtils.ConnectAndGet(conn => {
                var query =
                    "SELECT * FROM engine4_activity_actions WHERE object_id=@objectId and object_type=@objectType";
                if (type != null) query += " and type=@type";

                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("objectId", objectId);
                cmd.Parameters.AddWithValue("objectType", objectType);
                if (type != null) cmd.Parameters.AddWithValue("type", type);
                var items = DbUtils.GetRecords<ActivityAction>(cmd);
               
                return items;
            });
        }

        public static IEnumerable<ActivityAction> GetBySubjectId(uint subjectId, string subjectType,
            bool getAttachments = true) {
            return DbUtils.ConnectAndGet(conn => {
                var query =
                    "SELECT * FROM engine4_activity_actions WHERE subject_id=@subjectId and subject_type=@subjectType";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("subjectId", subjectId);
                cmd.Parameters.AddWithValue("subjectType", subjectType);
                var items = DbUtils.GetRecords<ActivityAction>(cmd);
               
                if (getAttachments) {
                    // We have the items records, now fetch all attachments belonging to any
                    // of the items and nest them inside each item's object

                    var ids = items.Select(x => x.action_id).ToArray();
                    var attachments = ActivityAttachments.Get(ids).ToLookup(x => x.action_id);
                    foreach (var item in items)
                        item.Attachments = !attachments.Contains(item.action_id)
                            ? new List<ActivityAttachment>()
                            : attachments[item.action_id].ToList();
                }

                return items;
            });
        }

        public static IEnumerable<ActivityAction> GetBySubjectOrObjectId(uint id, string type,
DateTime endDate,int limit = 50,bool getAttachments = true) {
            return DbUtils.ConnectAndGet(conn => {
                var query =
                    $"SELECT * FROM engine4_activity_actions WHERE ((subject_id=@id and subject_type=@type) or (object_id=@id and object_type=@type)) and date<@end_date order by action_id desc limit {limit}";
                
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("id", id);
                cmd.Parameters.AddWithValue("type", type);
                cmd.Parameters.AddWithValue("end_date", endDate);
                var items = DbUtils.GetRecords<ActivityAction>(cmd);
               
                if (getAttachments) {
                    // We have the items records, now fetch all attachments belonging to any
                    // of the items and nest them inside each item's object

                    var ids = items.Select(x => x.action_id).ToArray();
                    var attachments = ActivityAttachments.Get(ids).ToLookup(x => x.action_id);
                    foreach (var item in items)
                        item.Attachments = !attachments.Contains(item.action_id)
                            ? new List<ActivityAttachment>()
                            : attachments[item.action_id].ToList();
                }

                return items;
            });
        }

        public static IEnumerable<ulong> GetIdsBySubjectOrObjectId(uint id, string type,
DateTime endDate, int limit = 50, bool getAttachments = true)
        {
            return DbUtils.ConnectAndGet(conn => {
                var query =
                    $"SELECT action_id FROM engine4_activity_actions WHERE ((subject_id=@id and subject_type=@type) or (object_id=@id and object_type=@type)) and date<@end_date order by action_id desc limit {limit}";

                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("id", id);
                cmd.Parameters.AddWithValue("type", type);
                cmd.Parameters.AddWithValue("end_date", endDate);
                var items = DbUtils.GetRecords<ulong>(cmd);
               
                
                return items;
            });
        }

        public static List<ActivityAction> GetAll(bool getAttachments = true) {
            return DbUtils.ConnectAndGet(conn => {
                var query = "SELECT * FROM engine4_activity_actions";

                var cmd = new MySqlCommand(query, conn);
                var items = DbUtils.GetRecords<ActivityAction>(cmd);

                if (getAttachments) {
                    // We have the items records, now fetch all attachments belonging to any
                    // of the items and nest them inside each item's object

                    var ids = items.Select(x => x.action_id).ToArray();
                    var attachments = ActivityAttachments.Get(ids).ToLookup(x => x.action_id);
                    foreach (var item in items)
                        item.Attachments = !attachments.Contains(item.action_id)
                            ? new List<ActivityAttachment>()
                            : attachments[item.action_id].ToList();
                }

                return items;
            });
        }

        public static ActivityAction GetRefActivity(string type,ulong objectId, string objectType, ulong userId,
            bool getAttachments = true,DateTime creationDate = default(DateTime)) {
            Console.WriteLine($"sesrching for like activity to {objectType}:{objectId} by user {userId}");
            return DbUtils.ConnectAndGet(conn => {
                var query = "SELECT * FROM engine4_activity_actions WHERE type=@type and" +
                            " subject_id=@id and subject_type='user' and object_id=@objectId and object_type=@objectType";
                if(creationDate != default(DateTime))
                {
                    query += " AND date=@date";
                }
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("id", (uint) userId);
                cmd.Parameters.AddWithValue("objectId", (uint) objectId);
                cmd.Parameters.AddWithValue("objectType", objectType);
                cmd.Parameters.AddWithValue("type", type);
                cmd.Parameters.AddWithValue("date", creationDate);
                var items = DbUtils.GetRecords<ActivityAction>(cmd);

                if (getAttachments) {
                    // We have the items records, now fetch all attachments belonging to any
                    // of the items and nest them inside each item's object

                    var ids = items.Select(x => x.action_id).ToArray();
                    var attachments = ActivityAttachments.Get(ids).ToLookup(x => x.action_id);
                    foreach (var item in items)
                        item.Attachments = !attachments.Contains(item.action_id)
                            ? new List<ActivityAttachment>()
                            : attachments[item.action_id].ToList();
                }

                return items;
            }).FirstOrDefault();
        }

        public static ulong SetInnerActivity(ulong parentActionId, ulong innerActionId, string type) {
            return DbUtils.ConnectAndExecute(conn => {
                var query = "UPDATE engine4_activity_actions " +
                            "SET object_id=@innerActionId,object_type='activity_action',type=@type  " +
                            "WHERE action_id=@parentActionId";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("innerActionId", innerActionId);
                cmd.Parameters.AddWithValue("parentActionId", parentActionId);
                cmd.Parameters.AddWithValue("type", type + "_activity_action");
                var rc = cmd.ExecuteNonQuery();
                //delete previous attachments
                ActivityAttachments.DeleteForActivity(parentActionId);
                //set the attachment
                var attachment = new ActivityAttachment {
                    action_id = (uint) parentActionId,
                    id = (uint) innerActionId,
                    type = "activity_action",
                    mode = true
                };
                return ActivityAttachments.Add(attachment);
            });
        }

        public static ulong Add(ActivityAction newAction,bool addAttachment)
        {
            Console.WriteLine($"adding activity to table:{JsonConvert.SerializeObject(newAction)}");
            return DbUtils.ConnectAndExecute(conn => {
                var query = "INSERT INTO engine4_activity_actions(type,subject_type,subject_id,object_type,object_id,body,params,date,modified_date," +
                "attachment_count,comment_count,like_count,privacy,commentable,shareable,user_agent,publish_date) " +
                            $"VALUES (@type,@subject_type,@subject_id,@object_type,@object_id,@body,@params,@date,@modified_date," +
                            $"@attachment_count,@comment_count,@like_count,@privacy,@commentable,@shareable,@user_agent,@publish_date)";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("type", newAction.type);
                cmd.Parameters.AddWithValue("subject_type", newAction.subject_type);
                cmd.Parameters.AddWithValue("subject_id", newAction.subject_id);
                cmd.Parameters.AddWithValue("object_type", newAction.object_type);
                cmd.Parameters.AddWithValue("object_id", newAction.object_id);
                cmd.Parameters.AddWithValue("body", newAction.body);
                cmd.Parameters.AddWithValue("params", newAction.@params);
                cmd.Parameters.AddWithValue("date", newAction.date);
                cmd.Parameters.AddWithValue("modified_date", newAction.modified_date);
                cmd.Parameters.AddWithValue("attachment_count", newAction.attachment_count);
                cmd.Parameters.AddWithValue("comment_count", newAction.comment_count);
                cmd.Parameters.AddWithValue("like_count", newAction.like_count);
                cmd.Parameters.AddWithValue("privacy", newAction.privacy);
                cmd.Parameters.AddWithValue("commentable", newAction.commentable);
                cmd.Parameters.AddWithValue("shareable", newAction.shareable);
                cmd.Parameters.AddWithValue("user_agent", newAction.user_agent);
                cmd.Parameters.AddWithValue("publish_date", newAction.publish_date);
                var rc = cmd.ExecuteNonQuery();

                cmd.CommandText = "Select @@Identity";
                ulong actionId = (ulong)cmd.ExecuteScalar();
                newAction.action_id = (uint)actionId;
                if (addAttachment)
                {
                    newAction.Attachments[0].action_id = (uint)newAction.action_id;
                    newAction.Attachments[0].attachment_id = (uint)ActivityAttachments.Add(newAction.Attachments[0]);
                }

                return (ulong)newAction.action_id;
            });
        }

        public static ulong Update(ActivityAction newAction)
        {
            Console.WriteLine($"adding activity to table");
            return DbUtils.ConnectAndExecute(conn => {
                var query = "UPDATE engine4_activity_actions SET subject_type=@subject_type," +
                "subject_id=@subject_id,object_type=@object_type,object_id=@object_id,body=@body," +
                "params=@params,date=@date,modified_date=@modified_date,attachment_count=@attachment_count," +
                "comment_count=@comment_count,like_count=@like_count,privacy=@privacy,commentable=@commentable," +
                "shareable=@shareable,user_agent=@user_agent,publish_date=@publish_date,type=@type " +
                            $"WHERE action_id=@action_id";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("action_id", newAction.action_id);
                cmd.Parameters.AddWithValue("subject_type", newAction.subject_type);
                cmd.Parameters.AddWithValue("subject_id", newAction.subject_id);
                cmd.Parameters.AddWithValue("object_type", newAction.object_type);
                cmd.Parameters.AddWithValue("object_id", newAction.object_id);
                cmd.Parameters.AddWithValue("body", newAction.body);
                cmd.Parameters.AddWithValue("params", newAction.@params);
                cmd.Parameters.AddWithValue("date", newAction.date);
                cmd.Parameters.AddWithValue("modified_date", newAction.modified_date);
                cmd.Parameters.AddWithValue("attachment_count", newAction.attachment_count);
                cmd.Parameters.AddWithValue("comment_count", newAction.comment_count);
                cmd.Parameters.AddWithValue("like_count", newAction.like_count);
                cmd.Parameters.AddWithValue("privacy", newAction.privacy);
                cmd.Parameters.AddWithValue("commentable", newAction.commentable);
                cmd.Parameters.AddWithValue("shareable", newAction.shareable);
                cmd.Parameters.AddWithValue("user_agent", newAction.user_agent);
                cmd.Parameters.AddWithValue("publish_date", newAction.publish_date);
                cmd.Parameters.AddWithValue("type", newAction.type);
                var rc = cmd.ExecuteNonQuery();
                
                return (ulong)rc;
            });
        }

        public static ulong Delete(uint actionId, bool deleteAttachments)
        {
            Console.WriteLine($"deleting activity {actionId} from table");
            return DbUtils.ConnectAndExecute(conn => {
                var query =
                    "DELETE FROM engine4_activity_actions WHERE action_id=@actionId";
                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("actionId", actionId);
                if (deleteAttachments) { 
                    ActivityAttachments.DeleteForActivity(actionId);
                }
                var rc = cmd.ExecuteNonQuery();
                return (ulong)rc;
            });
        }

    }


    
     
}