namespace KoobecaFeedController.DAL.Models {
    /*
    +----------------+---------------------+------+-----+---------+----------------+
    | Field          | Type                | Null | Key | Default | Extra          |
    +----------------+---------------------+------+-----+---------+----------------+
    | file_id        | int(10) unsigned    | NO   | PRI | NULL    | auto_increment |
    | parent_file_id | int(10) unsigned    | YES  | MUL | NULL    |                |
    | type           | varchar(16)         | YES  |     | NULL    |                |
    | parent_type    | varchar(32)         | YES  | MUL | NULL    |                |
    | parent_id      | int(10) unsigned    | YES  |     | NULL    |                |
    | user_id        | int(10) unsigned    | YES  | MUL | NULL    |                |
    | creation_date  | datetime            | NO   |     | NULL    |                |
    | modified_date  | datetime            | NO   |     | NULL    |                |
    | service_id     | int(10) unsigned    | NO   | MUL | 1       |                |
    | storage_path   | varchar(255)        | NO   |     | NULL    |                |
    | extension      | varchar(8)          | NO   |     | NULL    |                |
    | name           | varchar(255)        | YES  |     | NULL    |                |
    | mime_major     | varchar(64)         | NO   |     | NULL    |                |
    | mime_minor     | varchar(64)         | NO   |     | NULL    |                |
    | size           | bigint(20) unsigned | NO   |     | NULL    |                |
    | hash           | varchar(64)         | NO   |     | NULL    |                |
    | params         | text                | YES  |     | NULL    |                |
    +----------------+---------------------+------+-----+---------+----------------+
     */


    public class Storage {
        public uint file_id { get; set; }
        public string storage_path { get; set; }
        public string @params { get; set; }
    }
}