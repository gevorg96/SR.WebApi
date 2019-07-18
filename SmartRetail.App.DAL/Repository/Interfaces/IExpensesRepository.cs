using System;
using System.Collections.Generic;
using SmartRetail.App.DAL.Entities;

namespace SmartRetail.App.DAL.Repository
{
    public interface IExpensesRepository
    {
        IEnumerable<Expenses> GetExpenses(int businessId, int? shopId, DateTime from, DateTime to);
    }
}