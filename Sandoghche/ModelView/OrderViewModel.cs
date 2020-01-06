using System;
using System.Collections.Generic;
using System.Text;
using Sandoghche.Models;

namespace Sandoghche.ModelView
{
   public class OrderViewModel
    {
        public OrderViewModel()
        {
            Order = new Order();
            OrderDetail = new List<OrderDetailViewModel>();
        }
        public Order Order { get; set; }
        public List<OrderDetailViewModel> OrderDetail { get; set; }

    }


    public class OrderDetailViewModel
    {
        public int ProductId { get; set; }
        public string ProductText { get; set; }
        public int CategoryId { get; set; }
        public int Number { get; set; }
        public double Price { get; set; }
        public double TotalPrice { get; set; }


    }

}
