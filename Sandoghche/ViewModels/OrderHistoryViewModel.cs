using System;
using System.Collections.Generic;
using System.Text;

namespace Sandoghche.ViewModels
{
    public class OrderHistoryViewModel
    {
        public int RowNumber { get; set; }
        public int ReceiptNumber { get; set; }
        public int OrderId { get; set; }
        public string ClientName { get; set; }
        public double FinalPayment { get; set; }
        public DateTime DateCreated { get; internal set; }
        public int EditedLogId { get; set; }


    }
}
