using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Sandoghche.Models
{
    public class SandoghcheSetting
    {
        [PrimaryKey]
        public int Id { get; set; }
        public double Tax1 { get; set; }
        public double Tax2 { get; set; }

        public string CompanyName { get; set; }
        public TimeSpan ResetReceiptTime { get; set; }
        public string QuoteText { get; set; }

        public int ReceiptNumberStartFrom { get; set; }




    }
}
