using System;

namespace SmartRetail.App.Web.Models.ViewModel.Expenses
{
    public class ExpensesRequestViewModel
    {
        public int? shopId { get; set; }
        public DateTime from { get; set; }
        public DateTime to { get; set; }
    }
}