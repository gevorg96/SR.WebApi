using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartRetail.App.DAL.BLL.DataServices
{
    public interface IExpensesDataService
    {
        Task<Dictionary<string, decimal>> GetMonthExpensesAsync(int shopId);
    }
}
