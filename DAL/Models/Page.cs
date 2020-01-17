namespace KoobecaFeedController.DAL.Models {
    /*
     * +--------------------+---------------------------------------------------------------------------------------+------+-----+---------+----------------+
| Field              | Type                                                                                  | Null | Key | Default | Extra          |
+--------------------+---------------------------------------------------------------------------------------+------+-----+---------+----------------+
| page_id            | int(11) unsigned                                                                      | NO   | PRI | NULL    | auto_increment |
| title              | varchar(255)                                                                          | NO   |     | NULL    |                |
| page_url           | varchar(255)                                                                          | NO   |     | NULL    |                |
| body               | text                                                                                  | YES  |     | NULL    |                |
| overview           | text                                                                                  | YES  |     | NULL    |                |
| owner_id           | int(11) unsigned                                                                      | NO   | MUL | NULL    |                |
| package_id         | int(11) unsigned                                                                      | NO   | MUL | NULL    |                |
| category_id        | int(11) unsigned                                                                      | NO   | MUL | NULL    |                |
| profile_type       | int(11)                                                                               | NO   | MUL | 0       |                |
| photo_id           | int(10) unsigned                                                                      | NO   |     | 0       |                |
| price              | int(11)                                                                               | NO   |     | 0       |                |
| location           | text                                                                                  | YES  |     | NULL    |                |
| creation_date      | datetime                                                                              | NO   |     | NULL    |                |
| modified_date      | datetime                                                                              | NO   |     | NULL    |                |
| approved           | tinyint(1)                                                                            | NO   |     | NULL    |                |
| featured           | tinyint(1)                                                                            | NO   | MUL | NULL    |                |
| sponsored          | tinyint(1)                                                                            | NO   | MUL | 0       |                |
| view_count         | int(11) unsigned                                                                      | NO   |     | 1       |                |
| comment_count      | int(11) unsigned                                                                      | NO   |     | 0       |                |
| like_count         | int(11) unsigned                                                                      | NO   |     | 0       |                |
| foursquare_text    | text                                                                                  | YES  |     | NULL    |                |
| search             | tinyint(1)                                                                            | NO   | MUL | 1       |                |
| closed             | tinyint(4)                                                                            | NO   |     | 0       |                |
| declined           | tinyint(1)                                                                            | NO   |     | 0       |                |
| pending            | tinyint(1)                                                                            | NO   |     | 1       |                |
| aprrove_date       | datetime                                                                              | YES  |     | NULL    |                |
| draft              | tinyint(1) unsigned                                                                   | NO   |     | 1       |                |
| subcategory_id     | int(11)                                                                               | NO   |     | 0       |                |
| userclaim          | tinyint(1)                                                                            | NO   |     | 1       |                |
| offer              | tinyint(1)                                                                            | NO   |     | NULL    |                |
| email              | varchar(255)                                                                          | NO   |     | NULL    |                |
| website            | varchar(255)                                                                          | NO   |     | NULL    |                |
| phone              | varchar(255)                                                                          | NO   |     | NULL    |                |
| status             | enum('initial','trial','pending','active','cancelled','expired','overdue','refunded') | NO   | MUL | initial |                |
| all_post           | tinyint(1)                                                                            | NO   |     | 1       |                |
| payment_date       | datetime                                                                              | YES  |     | NULL    |                |
| expiration_date    | datetime                                                                              | YES  |     | NULL    |                |
| notes              | text                                                                                  | YES  |     | NULL    |                |
| gateway_id         | int(10) unsigned                                                                      | YES  | MUL | NULL    |                |
| gateway_profile_id | varchar(128)                                                                          | YES  |     | NULL    |                |
| networks_privacy   | mediumtext                                                                            | YES  |     | NULL    |                |
| subsubcategory_id  | int(11)                                                                               | NO   |     | NULL    |                |
| subpage            | tinyint(1)                                                                            | NO   |     | NULL    |                |
| parent_id          | int(11)                                                                               | NO   | MUL | 0       |                |
| member_title       | varchar(64)                                                                           | NO   |     | NULL    |                |
| page_cover         | int(11)                                                                               | NO   |     | 0       |                |
| follow_count       | int(11)                                                                               | NO   |     | NULL    |                |
| fbpage_url         | varchar(255)                                                                          | YES  |     | NULL    |                |
| badge_id           | int(11)                                                                               | NO   |     | 0       |                |
+--------------------+---------------------------------------------------------------------------------------+------+-----+---------+----------------+
     */

    public class Page : IActivityObject,IFollowedSource
    {
        public uint page_id { set; get; }
        public string title { set; get; }
        public uint photo_id { set; get; }
        public uint owner_id { set; get; }
        public int follow_count { set; get; }

        void IFollowedSource.DecreaseFollow()
        {
            follow_count--;
        }

        void IFollowedSource.IncreasFollow()
        {
            follow_count++;
        }

        string IActivityObject.GetObjectTitle() {
            return title;
        }

        uint IActivityObject.GetStorageFileId() {
            return photo_id;
        }

        public string GetObjectDesctiption()
        {
            return "";
        }
    }
}