namespace KoobecaFeedController.DAL.Models {
    // ReSharper disable once CommentTypo

    /*
    +------------------+--------------+------+-----+---------+----------------+
    | Field            | Type         | Null | Key | Default | Extra          |
    +------------------+--------------+------+-----+---------+----------------+
    | word_id          | int(11)      | NO   | PRI | NULL    | auto_increment |
    | title            | varchar(100) | NO   |     | NULL    |                |
    | color            | varchar(10)  | NO   |     | NULL    |                |
    | background_color | varchar(10)  | NO   |     | NULL    |                |
    | style            | varchar(10)  | NO   |     | NULL    |                |
    | params           | text         | YES  |     | NULL    |                |
    +------------------+--------------+------+-----+---------+----------------+
     */

    public class WordStyling {
        public int word_id { get; set; }
        public string title { get; set; }
        public string color { get; set; }
        public string background_color { get; set; }
        public string style { get; set; }
        public string @params { get; set; }
    }
}