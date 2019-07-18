using System;
using System.Collections.Generic;

namespace SmartRetail.App.Web.Models.ViewModel.Expenses
{
    public class ExpensesViewModel
    {
        public DateTime reportDate { get; set; }
        public decimal totalSum { get; set; }
        public IEnumerable<ExpensesValueViewModel> expenses { get; set; }
    }
}