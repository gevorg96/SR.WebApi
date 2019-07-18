using System;
using System.Collections.Generic;

namespace SmartRetail.App.Web.Models.ViewModel
{
    public class SalesViewModel
    {
        public int billNumber { get; set; }
        public DateTime? reportDate { get; set; }

        public IEnumerable<SalesProductViewModel> products;

        public decimal totalSum { get; set; }
        public decimal discount { get; set; }

    }
}
