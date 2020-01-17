// ReSharper disable InconsistentNaming

namespace KoobecaFeedController.DAL.Models {
    /*
     * +-------------------+------------------+------+-----+---------+-------+
     * | Field             | Type             | Null | Key | Default | Extra |
     * +-------------------+------------------+------+-----+---------+-------+
     * | resource_id       | int(11) unsigned | NO   | PRI | NULL    |       |
     * | user_id           | int(11) unsigned | NO   | PRI | NULL    |       |
     * | active            | tinyint(1)       | NO   |     | 0       |       |
     * | resource_approved | tinyint(1)       | NO   |     | 0       |       |
     * | user_approved     | tinyint(1)       | NO   |     | 0       |       |
     * | message           | text             | YES  |     | NULL    |       |
     * | description       | text             | YES  |     | NULL    |       |
     * +-------------------+------------------+------+-----+---------+-------+
     */
    public class UserMembership {
        public uint resource_id { get; set; }
        public uint user_id { get; set; }
        public bool active { get; set; }
        public bool resource_approved { get; set; }
        public bool user_approved { get; set; }
        public string message { get; set; }
        public string description { get; set; }
    }
}