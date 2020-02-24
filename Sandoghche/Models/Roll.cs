using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sandoghche.Models
{
    [Table("Rolls")]
    public class Roll
    {
        [PrimaryKey, AutoIncrement]
        public int RollId { get; set; }
        public string RollName { get; set; }

    }
}