using System;
using System.Collections.Generic;

namespace SmartRetail.App.Web.Models.ViewModel.Orders
{
    public class OrderCreateViewModel
    {
        public int id { get; set; }
        public int shopId { get; set; }
        public DateTime reportDate { get; set; }
        public List<OrderRowViewModel> products { get; set; }
        public OrderCreateViewModel()
        {
            products = new List<OrderRowViewModel>();
        }
    }
}
