using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartRetail.App.DAL.Entities;

namespace SmartRetail.App.DAL.Repository.Interfaces
{
    public interface IExpensesRepository
    {
        Task<IEnumerable<Expense>> GetExpensesAsync(int businessId, int? shopId, DateTime from, DateTime to);
        Task<int> AddExpenses(Expense exp);
        Task<Expense> GetByDateAndShopBusiness(int businessId, DateTime reportDate, int? shopid = null);
        Task<Expense> GetByIdAsync(int id);
    }
}