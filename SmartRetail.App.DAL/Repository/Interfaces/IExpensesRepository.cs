using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartRetail.App.DAL.Entities;

namespace SmartRetail.App.DAL.Repository
{
    public interface IExpensesRepository
    {
        Task<IEnumerable<Expenses>> GetExpensesAsync(int businessId, int? shopId, DateTime from, DateTime to);
        Task<int> AddExpenses(Expenses exp);
        Task<Expenses> GetByDateAndShopBusiness(int businessId, DateTime reportDate, int? shopid = null);
        Task<Expenses> GetByIdAsync(int id);
    }
}