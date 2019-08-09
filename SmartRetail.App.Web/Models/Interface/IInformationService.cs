using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SmartRetail.App.DAL.Entities;

namespace SmartRetail.App.Web.Models.Interface
{
    public interface IInformationService
    {
        Task<JObject> GetDailyData(int whouse, UserProfile user);
        Task<JObject> GetMonthData(int whouse, UserProfile user);
        JObject GetStocks(int whouse);
        JObject GetExpenses(int whouse);
        Task<JObject> GetStocksAsync(int whouse, UserProfile user);
        Task<JObject> GetExpensesAsync(int whouse, UserProfile user);
    }
}
