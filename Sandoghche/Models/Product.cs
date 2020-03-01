using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sandoghche.Models
{
    [Table("Products")]
    public class Product
    {
        [PrimaryKey, AutoIncrement]
        public int ProductId { get; set; }
        public string ProductText { get; set; }
        public double ProductPrice { get; set; }
        public int CategoryId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public string CreatedDate
        {
            get
            {
                return this.createdDate != null
                   ? Convert.ToDateTime(createdDate).ToString("yyyy-MM-dd HH:mm:ss")
                   : DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }

            set
            {
                if (this.createdDate != null)
                    this.createdDate = Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss");
                else
                    this.createdDate = value;
            }
        }

        private string createdDate = null;




    }
}
