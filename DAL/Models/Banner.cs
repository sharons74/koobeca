using System;
using System.Collections.Generic;
using System.Text;
/*
+------------------+--------------+------+-----+---------+----------------+
| Field            | Type         | Null | Key | Default | Extra          |
+------------------+--------------+------+-----+---------+----------------+
| banner_id        | int(11)      | NO   | PRI | NULL    | auto_increment |
| startdate        | date         | NO   |     | NULL    |                |
| enddate          | date         | NO   |     | NULL    |                |
| file_id          | int(11)      | NO   |     | NULL    |                |
| enabled          | tinyint(4)   | NO   |     | NULL    |                |
| color            | varchar(30)  | YES  |     | NULL    |                |
| background_color | varchar(30)  | YES  |     | NULL    |                |
| gradient         | varchar(200) | YES  |     | NULL    |                |
| order            | tinyint(4)   | NO   |     | NULL    |                |
| highlighted      | tinyint(4)   | NO   |     | NULL    |                |
+------------------+--------------+------+-----+---------+----------------+
 */
namespace KoobecaFeedController.DAL.Models
{
    
    public class Banner
    {
        public int file_id { get; set; }
        public sbyte highlighted { get; set; }
        public string color { get; set; }
        public string background_color { get; set; }
    }
}
