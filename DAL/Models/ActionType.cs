namespace KoobecaFeedController.DAL.Models {
    /*
+-----------------+-------------+------+-----+---------+-------+
| Field           | Type        | Null | Key | Default | Extra |
+-----------------+-------------+------+-----+---------+-------+
| type            | varchar(64) | NO   | PRI | NULL    |       |
| module          | varchar(32) | NO   |     | NULL    |       |
| body            | text        | NO   |     | NULL    |       |
| enabled         | tinyint(1)  | NO   |     | 1       |       |
| displayable     | tinyint(1)  | NO   |     | 3       |       |
| attachable      | tinyint(1)  | NO   |     | 1       |       |
| commentable     | tinyint(1)  | NO   |     | 1       |       |
| shareable       | tinyint(1)  | NO   |     | 1       |       |
| editable        | tinyint(1)  | NO   |     | 0       |       |
| is_generated    | tinyint(1)  | NO   |     | 1       |       |
| is_grouped      | tinyint(1)  | NO   |     | 0       |       |
| is_object_thumb | tinyint(1)  | NO   |     | 0       |       |
+-----------------+-------------+------+-----+---------+-------+
*/
    public class ActionType {
        public string type { get; set; }
        public string module { get; set; }
        public string body { get; set; }
        public bool enabled { get; set; }
        public bool displayable { get; set; }
        public bool attachable { get; set; }
        public bool commentable { get; set; }
        public bool shareable { get; set; }
        public bool editable { get; set; }
        public bool is_generated { get; set; }
        public bool is_grouped { get; set; }
        public bool is_object_thumb { get; set; }
    }
}