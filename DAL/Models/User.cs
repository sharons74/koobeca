using System;

namespace KoobecaFeedController.DAL.Models {
    /*
    +---------------------+----------------------+------+-----+---------------------+----------------+
    | Field               | Type                 | Null | Key | Default             | Extra          |
    +---------------------+----------------------+------+-----+---------------------+----------------+
    | user_id             | int(11) unsigned     | NO   | PRI | NULL                | auto_increment |
    | email               | varchar(128)         | NO   | UNI | NULL                |                |
    | username            | varchar(128)         | YES  | UNI | NULL                |                |
    | displayname         | varchar(128)         | NO   |     |                     |                |
    | photo_id            | int(11) unsigned     | NO   |     | 0                   |                |
    | status              | text                 | YES  |     | NULL                |                |
    | status_date         | datetime             | YES  |     | NULL                |                |
    | password            | char(32)             | NO   |     | NULL                |                |
    | salt                | char(64)             | NO   |     | NULL                |                |
    | locale              | varchar(16)          | NO   |     | auto                |                |
    | language            | varchar(8)           | NO   |     | en_US               |                |
    | timezone            | varchar(64)          | NO   |     | America/Los_Angeles |                |
    | search              | tinyint(1)           | NO   | MUL | 1                   |                |
    | show_profileviewers | tinyint(1)           | NO   |     | 1                   |                |
    | level_id            | int(11) unsigned     | NO   |     | NULL                |                |
    | invites_used        | int(11) unsigned     | NO   |     | 0                   |                |
    | extra_invites       | int(11) unsigned     | NO   |     | 0                   |                |
    | enabled             | tinyint(1)           | NO   | MUL | 1                   |                |
    | verified            | tinyint(1)           | NO   |     | 0                   |                |
    | approved            | tinyint(1)           | NO   |     | 1                   |                |
    | creation_date       | datetime             | NO   | MUL | NULL                |                |
    | creation_ip         | varbinary(16)        | NO   |     | NULL                |                |
    | modified_date       | datetime             | NO   |     | NULL                |                |
    | lastlogin_date      | datetime             | YES  |     | NULL                |                |
    | lastlogin_ip        | varbinary(16)        | YES  |     | NULL                |                |
    | update_date         | int(11)              | YES  |     | NULL                |                |
    | member_count        | smallint(5) unsigned | NO   | MUL | 0                   |                |
    | view_count          | int(11) unsigned     | NO   |     | 0                   |                |
    | comment_count       | int(11) unsigned     | NO   |     | 0                   |                |
    | like_count          | int(11) unsigned     | NO   |     | 0                   |                |
    | coverphoto          | int(11) unsigned     | NO   |     | 0                   |                |
    | coverphotoparams    | varchar(265)         | YES  |     | NULL                |                |
    | view_privacy        | varchar(24)          | NO   |     | everyone            |                |
    | seao_locationid     | int(11)              | NO   |     | NULL                |                |
    | location            | varchar(264)         | NO   |     | NULL                |                |
    +---------------------+----------------------+------+-----+---------------------+----------------+
     */

    public class User : IActivityObject {
        public uint user_id { get; set; }
        public string email { get; set; }
        public string displayname { get; set; }
        public uint photo_id { get; set; }

        public string GetObjectDesctiption()
        {
            return "";
        }

        string IActivityObject.GetObjectTitle() {
            return displayname;
        }

        uint IActivityObject.GetStorageFileId() {
            return photo_id;
        }
    }

    public class UserFull
    {
        public uint user_id { get; set; }
        public string email { get; set; }
        public string username { get; set; }
        public string displayname { get; set; }
        public uint photo_id { get; set; }
        public string status { get; set; }
        public DateTime status_date { get; set; }
        //public string password { get; set; }
        //public string salt { get; set; }
        public string locale { get; set; }
        public string language { get; set; }
        public string timezone { get; set; }
        //public byte search { get; set; }
        public bool show_profileviewers { get; set; }
        public uint level_id { get; set; }
        public uint invites_used { get; set; }
        public uint extra_invites { get; set; }
        public bool enabled { get; set; }
        public bool verified { get; set; }
        public bool approved { get; set; }
        public DateTime creation_date { get; set; }
        //public byte[] creation_ip { get; set; }
        public DateTime modified_date { get; set; }
        public DateTime lastlogin_date { get; set; }
        //public byte[] lastlogin_ip { get; set; }
        public DateTime update_date { get; set; }
        public uint member_count { get; set; }
        public uint view_count { get; set; }
        public uint comment_count { get; set; }
        public uint like_count { get; set; }
        public uint coverphoto { get; set; }
        public string coverphotoparams { get; set; }
        public string view_privacy { get; set; }
        public int seao_locationid { get; set; }
        public string location { get; set; }
    }
}