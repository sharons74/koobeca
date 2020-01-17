// ReSharper disable InconsistentNaming

using KoobecaFeedController.DAL.Adapters;

namespace KoobecaFeedController.DAL.Models {
    /*
     * +---------------+------------------+------+-----+---------+----------------+
     * | Field         | Type             | Null | Key | Default | Extra          |
     * +---------------+------------------+------+-----+---------+----------------+
     * | attachment_id | int(11) unsigned | NO   | PRI | NULL    | auto_increment |
     * | action_id     | int(11) unsigned | NO   | MUL | NULL    |                |
     * | type          | varchar(64)      | NO   | MUL | NULL    |                |
     * | id            | int(11) unsigned | NO   |     | NULL    |                |
     * | mode          | tinyint(1)       | NO   |     | 0       |                |
     * +---------------+------------------+------+-----+---------+----------------+
    */
    public class ActivityAttachment {
        public uint attachment_id { get; set; }
        public uint action_id { get; set; }
        public string type { get; set; }
        public uint id { get; set; }
        public bool mode { get; set; }

        public IActivityObject GetActivityObject()
        {
            switch (type)
            {
                case "core_link":
                    return CoreLinks.GetById(id);
                case "album_photo":
                    return AlbumPhotos.GetById(id);
                case "video":
                    return Videos.GetById(id);
                default:
                    return null;
            }
        }
    }
}