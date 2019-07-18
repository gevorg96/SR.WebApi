using System;
using System.Collections.Generic;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.Web.Models.ViewModel;
using SmartRetail.App.Web.Models.ViewModel.Expenses;

namespace SmartRetail.App.Web.Models.Interface
{
    public interface IExpensesService
    {
        IEnumerable<ExpensesViewModel> GetExpenses(UserProfile user, int? shopId, DateTime from, DateTime to);
    }
}