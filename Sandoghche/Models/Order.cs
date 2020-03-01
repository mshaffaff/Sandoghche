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

        //public string DateCreated
        //{
        //    get
        //    {
        //        return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //    }

        //    set { this.dateCreated = value; }
        //}

        //private string dateCreated = null;


        public string DateCreated
        {
            get
            {
                return this.dateCreated!=null
                   ? Convert.ToDateTime(dateCreated).ToString("yyyy-MM-dd HH:mm:ss")
                   : DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }

            set {
                if (this.dateCreated != null)
                    this.dateCreated = Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss");
                else
                    this.dateCreated = value;
            }
        }

        private string dateCreated = null;




        public DateTime? EditedTime { get; set; }
        public bool isEdited { get; set; }

        public bool isDeleted { get; set; }
        public DateTime? DeletedTime { get; set; }




        [OneToMany("OrderId")] 
        public List<OrderDetail> OrderDetails { get; set; }











    }
    
}
