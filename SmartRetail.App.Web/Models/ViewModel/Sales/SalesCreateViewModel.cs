using System;
using System.Collections.Generic;

namespace SmartRetail.App.Web.Models.ViewModel.Sales
{
    public class SalesCreateViewModel
    {
        public int shopId { get; set; }
        public DateTime reportDate { get; set; }
        public IEnumerable<SalesProductRowViewModel> products { get; set; }
    }
}
