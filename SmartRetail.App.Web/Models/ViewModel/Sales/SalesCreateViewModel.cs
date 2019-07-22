using System;
using System.Collections.Generic;

namespace SmartRetail.App.Web.Models.ViewModel.Sales
{
    public class SalesCreateViewModel
    {
        public int shopId { get; set; }
        public DateTime reportDate { get; set; }
        public decimal totalDiscount { get; set; }
        public decimal totalSum { get; set; }
        public IEnumerable<SalesProductRowViewModel> products { get; set; }
    }
}
