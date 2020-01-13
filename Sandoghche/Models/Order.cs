using SQLite;
using SQLiteNetExtensions.Attributes;

using System;
using System.Collections.Generic;
using System.Text;

namespace Sandoghche.Models
{
    [Table("Orders")]
    public class Order
    {
        [PrimaryKey, AutoIncrement]
        public int OrderId { get; set; }
        public int ReceiptNumber { get; set; }
        public int ClientId { get; set; }

        public double Tax1 { get; set; }
        public double Tax2 { get; set; }

        public double DeliveryFee { get; set; }

        public short ServiceType { get; set; }
        public double ServicePercent { get; set; }
        public double TotalServiceFee { get; set; }

        public short DiscountType { get; set; }
        public double DiscountPercent { get; set; }
        public double TotalDiscount { get; set; }

        public string Comment { get; set; }

        public short PaymentType { get; set; }
        public double TotalPrice { get; set; }

        public double FinalPayment { get; set; }

        public DateTime DateCreated
        {
            get
            {
                return this.dateCreated.HasValue
                   ? this.dateCreated.Value
                   : DateTime.Now;
            }

            set { this.dateCreated = value; }
        }

        private DateTime? dateCreated = null;



        [OneToMany("OrderId")] 
        public List<OrderDetail> OrderDetails { get; set; }











    }
    
}
