namespace KoobecaFeedController.DAL.Models {
    /*
    +-----------------+------------------+------+-----+---------------------+----------------+
    | Field           | Type             | Null | Key | Default             | Extra          |
    +-----------------+------------------+------+-----+---------------------+----------------+
    | photo_id        | int(11) unsigned | NO   | PRI | NULL                | auto_increment |
    | album_id        | int(11) unsigned | NO   | MUL | NULL                |                |
    | title           | varchar(128)     | NO   |     | NULL                |                |
    | description     | mediumtext       | NO   |     | NULL                |                |
    | creation_date   | datetime         | NO   |     | NULL                |                |
    | modified_date   | datetime         | NO   |     | NULL                |                |
    | order           | int(11) unsigned | NO   |     | 0                   |                |
    | owner_type      | varchar(64)      | NO   | MUL | NULL                |                |
    | owner_id        | int(11) unsigned | NO   |     | NULL                |                |
    | file_id         | int(11) unsigned | NO   |     | NULL                |                |
    | view_count      | int(11) unsigned | NO   |     | 0                   |                |
    | comment_count   | int(11) unsigned | NO   |     | 0                   |                |
    | like_count      | int(11) unsigned | NO   |     | 0                   |                |
    | photo_hide      | tinyint(1)       | NO   |     | 0                   |                |
    | rating          | float            | NO   |     | NULL                |                |
    | seao_locationid | int(11)          | NO   | MUL | NULL                |                |
    | location        | varchar(264)     | NO   |     | NULL                |                |
    | date_taken      | datetime         | NO   |     | 0000-00-00 00:00:00 |                |
    | skip_photo      | tinyint(1)       | NO   |     | 0                   |                |
    | featured        | tinyint(1)       | NO   |     | 0                   |                |
    +-----------------+------------------+------+-----+---------------------+----------------+
     */

    public class AlbumPhoto : IActivityObject {
        public uint photo_id { set; get; }
        public uint album_id { set; get; }
        public string title { set; get; }
        public string description { set; get; }
        public object creation_date { set; get; }
        public object modified_date { set; get; }
        public uint order { set; get; }
        public string owner_type { set; get; }
        public uint owner_id { set; get; }
        public uint file_id { set; get; }
        public uint view_count { set; get; }
        public uint comment_count { set; get; }
        public uint like_count { set; get; }
        public bool photo_hide { set; get; }
        public float rating { set; get; }
        public int seao_locationid { set; get; }

        public string location { set; get; }

        public string GetObjectDesctiption()
        {
            return description;
        }

        string IActivityObject.GetObjectTitle() {
            return title;
        }

        uint IActivityObject.GetStorageFileId() {
            return file_id;
        }

        //public object date_taken { set; get; }
        //public bool skip_photo { set; get; }
        //public bool featured { set; get; }
    }
}