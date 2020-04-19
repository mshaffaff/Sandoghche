using System;
using System.Collections.Generic;
using System.Text;

namespace Sandoghche.ViewModels
{
    public class ProductCategoryPriceViewModel
    {
        public string ProductText { get; set; }
        public int ProductId { get; set; }
        public double ProductPrice { get; set; }
        public bool isDeleted { get; set; }
        public bool IsActive { get; set; }
        public double ProductAmount { get; set; }
        public string CategoryText { get; set; }


    }
}
