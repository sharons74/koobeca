using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Newtonsoft.Json;

// ReSharper disable InconsistentNaming

namespace KoobecaFeedController.DAL.Models {
    // ReSharper disable once CommentTypo
    /*
     * +------------------+-----------------------+------+-----+---------+----------------+
     * | Field            | Type                  | Null | Key | Default | Extra          |
     * +------------------+-----------------------+------+-----+---------+----------------+
     * | action_id        | int(11) unsigned      | NO   | PRI | NULL    | auto_increment |
     * | type             | varchar(64)           | NO   |     | NULL    |                |
     * | subject_type     | varchar(24)           | NO   | MUL | NULL    |                |
     * | subject_id       | int(11) unsigned      | NO   |     | NULL    |                |
     * | object_type      | varchar(24)           | NO   | MUL | NULL    |                |
     * | object_id        | int(11) unsigned      | NO   |     | NULL    |                |
     * | body             | blob                  | YES  |     | NULL    |                |
     * | params           | text                  | YES  |     | NULL    |                |
     * | date             | datetime              | NO   |     | NULL    |                |
     * | modified_date    | datetime              | NO   |     | NULL    |                |
     * | attachment_count | smallint(3) unsigned  | NO   |     | 0       |                |
     * | comment_count    | mediumint(5) unsigned | NO   |     | 0       |                |
     * | like_count       | mediumint(5) unsigned | NO   |     | 0       |                |
     * | privacy          | varchar(500)          | YES  |     | NULL    |                |
     * | commentable      | tinyint(1)            | NO   |     | 1       |                |
     * | shareable        | tinyint(1)            | NO   |     | 1       |                |
     * | user_agent       | text                  | YES  |     | NULL    |                |
     * | publish_date     | datetime              | YES  |     | NULL    |                |
     * +------------------+-----------------------+------+-----+---------+----------------+
     */
    public class ActivityAction {
        private Dictionary<string, object> _ParamMap;

        public uint action_id { get; set; }
        public string type { get; set; }

        public static ActivityAction CreateReferenceAction(ActivityAction origActivity, string type,uint subjectId) {
            var action = new ActivityAction();
            action.date = action.modified_date = DateTime.Now;
            action.object_id = origActivity.action_id;
            action.object_type = "activity_action";
            action.privacy = origActivity.privacy == "everyone" ? "everyone" : null;
            action.type = type;
            action.subject_id = subjectId;
            action.subject_type = "user";
            if (type == "share")
            {
                action.@params = $"{{\"owner\":\"user_{subjectId}\"}}";
            }
            else
            {
                action.@params = $"{{\"owner\":\"{origActivity.subject_type}_{origActivity.subject_id}\"}}";
            }
            action.Attachments = new List<ActivityAttachment>();
            action.attachment_count = 1;
            var attachment = new ActivityAttachment() { id = (uint)origActivity.action_id, mode = true, type = "activity_action" };
            action.Attachments.Add(attachment);
            action.commentable = action.shareable = action.type == "share"; //only share ref activity has this
            return action;
        }

        public string subject_type { get; set; }
        public uint subject_id { get; set; }
        public string object_type { get; set; }
        public uint object_id { get; set; }
        public byte[] body { get; set; }
        public string @params { get; set; }
        public DateTime date { get; set; }
        public DateTime modified_date { get; set; }
        public uint attachment_count { get; set; }
        public uint comment_count { get; set; }
        public uint like_count { get; set; }
        public string privacy { get; set; }
        public bool commentable { get; set; }
        public bool shareable { get; set; }
        public string user_agent { get; set; }
        public DateTime? publish_date { get; set; }

        [MySqlIgnore]
        public List<ActivityAttachment> Attachments { get; set; }

        [MySqlIgnore]
        public List<uint> RelatedActionIds { get; set; }

        [MySqlIgnore]
        [JsonIgnore]
        public Dictionary<string, object> ParamMap {
            get {
                if (@params == null || @params == "[]") return null;
                lock (this) {
                    if (_ParamMap == null)
                        try {
                            _ParamMap = JsonConvert.DeserializeObject<Dictionary<string, object>>(@params);
                        }
                        catch (Exception e) {
                            //TODO log this
                            _ParamMap = new Dictionary<string, object>(); //so next time we won't try to parse json
                        }

                    return _ParamMap;
                }
            }
        }

        [MySqlIgnore]
        [JsonIgnore]
        public string BodyStr {
            get {
                if (body == null) return string.Empty;
                return HttpUtility.HtmlDecode(Encoding.UTF8.GetString(body));
            }
            set
            {
                body = Encoding.UTF8.GetBytes(HttpUtility.HtmlEncode(value));
            }
        }
        
        public object GetSingleParam(string key) {
            if (ParamMap != null) {
                ParamMap.TryGetValue(key, out var val);
                return val;
            }

            return null;
        }
    }
}