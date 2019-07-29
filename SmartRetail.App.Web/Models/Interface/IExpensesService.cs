using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.Web.Models.ViewModel.Expenses;

namespace SmartRetail.App.Web.Models.Interface
{
    public interface IExpensesService
    {
        Task<IEnumerable<ExpensesViewModel>> GetExpenses(UserProfile user, int? shopId, DateTime from, DateTime to);
        Task<ExpensesViewModel> AddExpenses(UserProfile user, ExpensesViewModel model);
    }
}