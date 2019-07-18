using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SmartRetail.App.Web.Models.Interface
{
    public interface IInformationService
    {
        Task<JObject> GetDailyData(int whouse);
        Task<JObject> GetMonthData(int whouse);
        JObject GetStocks(int whouse);
        JObject GetExpenses(int whouse);
        JObject GetWareHouses();
    }
}
