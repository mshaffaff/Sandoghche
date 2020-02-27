using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sandoghche.Models
{
    [Table("UserRolls")]
    public class UserRoll
    {
        //[PrimaryKey,AutoIncrement]
        //public int Id { get; set; }
        public int UserId { get; set; }
        public int RollId { get; set; }

    }
}
