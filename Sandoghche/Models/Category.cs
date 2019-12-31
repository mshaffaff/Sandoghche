using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sandoghche.Models
{
    [Table("Categories")]
    public class Category
    {
        [PrimaryKey,AutoIncrement]
        public int CategoryId { get; set; }
        public string CategoryText { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }


    }
}
