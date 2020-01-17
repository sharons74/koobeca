using Newtonsoft.Json;
using System;
using System.Collections.Generic;

/*
+-------------------+------------------+------+-----+---------+----------------+
| Field             | Type             | Null | Key | Default | Extra          |
+-------------------+------------------+------+-----+---------+----------------+
| comment_id        | int(11) unsigned | NO   | PRI | NULL    | auto_increment |
| resource_id       | int(11) unsigned | NO   | MUL | NULL    |                |
| poster_type       | varchar(24)      | NO   | MUL | NULL    |                |
| poster_id         | int(11) unsigned | NO   |     | NULL    |                |
| body              | blob             | YES  |     | NULL    |                |
| creation_date     | datetime         | NO   |     | NULL    |                |
| like_count        | int(11) unsigned | NO   |     | 0       |                |
| params            | text             | YES  |     | NULL    |                |
| parent_comment_id | int(11)          | NO   |     | 0       |                |
| attachment_type   | varchar(64)      | YES  |     | NULL    |                |
| attachment_id     | int(11)          | YES  |     | 0       |                |
+-------------------+------------------+------+-----+---------+----------------+
*/
namespace KoobecaFeedController.DAL.Models {
    public class Comment {
        private Dictionary<string, object> _params;

        public uint comment_id { get; set; }
        public uint resource_id { get; set; }
        public string poster_type { get; set; }
        public uint poster_id { get; set; }
        public byte[] body { get; set; }
        public DateTime creation_date { get; set; }
        public uint like_count { get; set; }
        public string @params { get; set; }
        public int parent_comment_id { get; set; }
        public string attachment_type { get; set; }
        public int attachment_id { get; set; }


        public T GetParam<T>(string key)
        {
            if(_params == null && !string.IsNullOrEmpty(@params))
            {
                _params = JsonConvert.DeserializeObject<Dictionary<string, object>>(@params);
            }
            if(_params != null)
            {
                _params.TryGetValue(key, out object res);
                return (T)res;
            }
            //else 
            return default(T);
        }
    }
}