using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartRetail.App.Web.Models.ViewModel.Orders
{
    public class OrderCreateViewModel
    {
        public int shopId { get; set; }
        public DateTime reportDate { get; set; }
        public List<OrderRowViewModel> products { get; set; }
        public OrderCreateViewModel()
        {
            products = new List<OrderRowViewModel>();
        }
    }
}
