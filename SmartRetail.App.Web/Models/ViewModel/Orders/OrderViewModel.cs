using System;
using System.Collections.Generic;

namespace SmartRetail.App.Web.Models.ViewModel.Orders
{
    public class OrderViewModel
    {
        public DateTime reportDate { get; set; }
        public List<OrderRowViewModel> products { get; set; }
        public OrderViewModel()
        {
            products = new List<OrderRowViewModel>();
        }
    }
}
