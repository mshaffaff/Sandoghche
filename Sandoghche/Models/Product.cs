using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sandoghche.Models
{
    [Table("Products")]
    public class Product
    {
        [PrimaryKey,AutoIncrement]
        public int ProductId { get; set; }
        public string ProductText { get; set; }
        public double ProductPrice { get; set; }
        public int CategoryId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime CreatedDate
    {
            get
            {
                return this.createdDate.HasValue
                   ? this.createdDate.Value
                   : DateTime.Now;
            }

            set { this.createdDate = value; }
        }

        private DateTime? createdDate = null;




    }
}
