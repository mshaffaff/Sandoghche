using System;
using System.Collections.Generic;
using System.Text;
using Sandoghche.Models;

namespace Sandoghche.ModelView
{

 
    public class OrderDetailForSearchViewModel
    {
        public int RowNumber { get; set; }
        public int ReceiptId { get; set; }
        public int OrderId { get; set; }
        public string ClientName { get; set; }
        public double FinalPayment { get; set; }
        public DateTime DateCreated { get; internal set; }
    }

    public class Test5
    {
        public int OrderId { get; set; }
        public double FinalPayment { get; set; }
    }

}
