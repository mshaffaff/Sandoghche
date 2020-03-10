using SQLite;
using SQLiteNetExtensions.Attributes;

using System;
using System.Collections.Generic;
using System.Text;


namespace Sandoghche.Models
{

    public class EditedOrdersLogs
    {
        [PrimaryKey,AutoIncrement]
        public int EditedLogId { get; set; }

        public int OrderId { get; set; }
        public int ReceiptNumber { get; set; }
        public int ClientId { get; set; }

        public double Tax1 { get; set; }
        public double Tax1Percent { get; set; }
        public double Tax2 { get; set; }
        public double Tax2Percent { get; set; }
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

        public string DateCreated { get; set; }


        public string EditedDate { get; set; }

        [OneToMany("EditedLogId")]
        public List<EditedOrderDetailsLogs> EditedOrderDetailsLogs { get; set; }


    }

}
