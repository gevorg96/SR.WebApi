using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartRetail.App.DAL.BLL.DataServices
{
    public class ExpensesDataService: IExpensesDataService
    {
        public ExpensesDataService()
        {
            
        }

        public async Task<Dictionary<string, decimal>> GetMonthExpensesAsync(int shopId)
        {
            
        }
    }
}
