using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sandoghche.Models
{
    [Table("OrderDetails")]
    public class OrderDetail
    {
        [PrimaryKey, AutoIncrement]
        public int DetailId { get; set; }
        
        [Ignore]
        public int RowNumber { get; set; }
        [ForeignKey(typeof(Order))]
        public int OrderId { get; set; }

        public int ProductId { get; set; }
       
        [Ignore]
        public string ProductText { get; set; }

        public int CategoryId { get; set; }

        public int Number { get; set; }
        public double Price { get; set; }
        public double TotalPrice { get; set; }

    }

}
