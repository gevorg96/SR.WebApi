using System;

namespace SmartRetail.App.Web.Models.ViewModel.Sales
{
    public class SalesRequestViewModel
    {
        public int? shopId { get; set; }
        public DateTime from { get; set; }
        public DateTime to { get; set; }
        public bool orderByDesc { get; set; }
    }
}