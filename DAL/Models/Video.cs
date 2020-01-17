using KoobecaFeedController.DAL.Adapters;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace KoobecaFeedController.DAL.Models {
    /*
    +-------------------+----------------------+------+-----+---------+----------------+
    | Field             | Type                 | Null | Key | Default | Extra          |
    +-------------------+----------------------+------+-----+---------+----------------+
    | video_id          | int(11) unsigned     | NO   | PRI | NULL    | auto_increment |
    | title             | varchar(100)         | NO   |     | NULL    |                |
    | description       | text                 | NO   |     | NULL    |                |
    | search            | tinyint(1)           | NO   | MUL | 1       |                |
    | owner_type        | varchar(128)         | NO   |     | NULL    |                |
    | owner_id          | int(11)              | NO   | MUL | NULL    |                |
    | parent_type       | varchar(128)         | YES  |     | NULL    |                |
    | parent_id         | int(11) unsigned     | YES  |     | NULL    |                |
    | creation_date     | datetime             | NO   | MUL | NULL    |                |
    | modified_date     | datetime             | NO   |     | NULL    |                |
    | view_count        | int(11) unsigned     | NO   | MUL | 0       |                |
    | comment_count     | int(11) unsigned     | NO   |     | 0       |                |
    | type              | varchar(32)          | NO   |     | NULL    |                |
    | code              | text                 | NO   |     | NULL    |                |
    | photo_id          | int(11) unsigned     | YES  |     | NULL    |                |
    | rating            | float                | NO   |     | NULL    |                |
    | category_id       | int(11) unsigned     | NO   |     | 0       |                |
    | status            | tinyint(1)           | NO   |     | NULL    |                |
    | file_id           | int(11) unsigned     | NO   |     | NULL    |                |
    | duration          | int(9) unsigned      | NO   |     | NULL    |                |
    | rotation          | smallint(5) unsigned | NO   |     | 0       |                |
    | main_channel_id   | int(11)              | YES  |     | NULL    |                |
    | subcategory_id    | int(11)              | YES  |     | NULL    |                |
    | subsubcategory_id | int(11)              | YES  |     | NULL    |                |
    | profile_type      | int(11)              | YES  |     | NULL    |                |
    | featured          | tinyint(1)           | NO   |     | 0       |                |
    | favourite_count   | int(11)              | NO   |     | 0       |                |
    | sponsored         | tinyint(1)           | NO   |     | 0       |                |
    | seao_locationid   | int(11)              | NO   |     | NULL    |                |
    | location          | varchar(264)         | NO   |     | NULL    |                |
    | networks_privacy  | mediumtext           | YES  |     | NULL    |                |
    | password          | char(32)             | YES  |     | NULL    |                |
    | like_count        | int(11)              | YES  |     | 0       |                |
    | synchronized      | int(11)              | YES  |     | 0       |                |
    +-------------------+----------------------+------+-----+---------+----------------+
     */

    public class Video : IActivityObject {
        //video_id,title,description,photo_id,file_id,code
        public uint video_id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public uint photo_id { get; set; }
        public uint file_id { get; set; }
        public string code { get; set; }
        public string type { get; set; }
        public uint comment_count { get; set; }
        public int like_count { get; set; }
        public int owner_id { get; set; }
        public string owner_type { get; set; }

        string IActivityObject.GetObjectTitle() {
            return title;
        }

        uint IActivityObject.GetStorageFileId() {
            return photo_id;
        }

        public string GetVideoUrl(string remoteStoragePrefix) {
            switch (type) {
                case "1":
                case "youtube":
                    return $"www.youtube.com/embed/{code}?wmode=opaque&autoplay=1";
                case "3":
                case "upload":
                case "mydevice":
                    return $"{remoteStoragePrefix}{Storages.GetByFileId(file_id).storage_path}";
                case "2":
                case "vimeo":
                    return
                        $"player.vimeo.com/video/{code}?title=0&amp;byline=0&amp;portrait=0&amp;wmode=opaque&amp;autoplay=1";
                case "4":
                case "dailymotion":
                    return $"www.dailymotion.com/embed/video/{code}?wmode=opaque&amp;autoplay=1";
                case "6":
                case "embedcode":
                case "iframely":
                   return string.IsNullOrEmpty(code) ? code : "";
                case "fb":
                    Dictionary<string, string> data = JsonConvert.DeserializeObject<Dictionary<string, string>>(code);
                    return data["video"];
                default:
                    return string.Empty;
            }
        }

        public uint GetVIdeoType() {
            switch (type) {
                case "1":
                case "youtube":
                    return 1;
                case "3":
                case "upload":
                case "mydevice":
                case "fb":
                    return 3;
                case "2":
                case "vimeo":
                    return 2;
                case "4":
                case "dailymotion":
                    return 4;
                case "6":
                case "embedcode":
                case "iframely":
                //case "fb":
                    return 6;
                default:
                    return 0;
            }
        }

        public string GetObjectDesctiption()
        {
            return description;
        }
    }
}