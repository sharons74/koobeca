

/*
+-------------+------------------+------+-----+---------+----------------+
| Field       | Type             | Null | Key | Default | Extra          |
+-------------+------------------+------+-----+---------+----------------+
| like_id     | int(11) unsigned | NO   | PRI | NULL    | auto_increment |
| resource_id | int(11) unsigned | NO   | MUL | NULL    |                |
| poster_type | varchar(16)      | NO   | MUL | NULL    |                |
| poster_id   | int (11) unsigned| NO   |     | NULL    |                |
| reaction    | varchar(64)      | YES  | MUL | like    |                |
+-------------+------------------+------+-----+---------+----------------+
*/
namespace KoobecaFeedController.DAL.Models {
    public class Like {
        public uint like_id { get; set; }
        public uint resource_id { get; set; }
        public string poster_type { get; set; }
        public uint poster_id { get; set; }
        public string reaction { get; set; }
    }
}