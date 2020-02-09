using System;
using System.Collections.Generic;
using System.Text;

namespace Sandoghche.ViewModels
{
    class OrderDetailForSearchViewModel
    {
        public int OrderId { get; set; }
        public int ReceiptNumber { get; set; }
        public double FinalPayment { get; set; }
        public DateTime DateCreated { get; set; }
        public string ClientName { get; set; }

    }
}
