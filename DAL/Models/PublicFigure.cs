using System;
using System.Collections.Generic;
using System.Text;

namespace KoobecaFeedController.DAL.Models
{
    /*
+---------+---------------------+------+-----+---------+-------+
| Field   | Type                | Null | Key | Default | Extra |
+---------+---------------------+------+-----+---------+-------+
| user_id | bigint(20) unsigned | NO   | PRI | NULL    |       |
+---------+---------------------+------+-----+---------+-------+
     */
    public class PublicFigure
    {
        public ulong user_id { get; set; }
    }
}
