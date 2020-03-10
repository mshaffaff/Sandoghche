using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sandoghche.Models
{
    public class EditedOrderDetailsLogs
    {
        [PrimaryKey,AutoIncrement]
        public int EditedOrderDetailLogId { get; set; }

        public int DetailId { get; set; }

        [Ignore]
        public int RowNumber { get; set; }

        [ForeignKey(typeof(EditedOrdersLogs))]
        public int EditedLogId { get; set; }
      
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
