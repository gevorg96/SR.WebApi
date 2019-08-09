using System.Collections.Generic;
using System.Threading.Tasks;
using SmartRetail.App.DAL.Entities;

namespace SmartRetail.App.DAL.BLL.DataServices
{
    public interface IExpensesDataService
    {
        Task<Dictionary<string, decimal>> GetMonthExpensesAsync(int shopId, UserProfile user);
    }
}
