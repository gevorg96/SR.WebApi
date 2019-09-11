using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SmartRetail.App.DAL.Entities;

namespace SmartRetail.App.Web.Models.Interface
{
    public interface IInformationService
    {
        Task<JObject> GetInfo(int whouse, UserProfile user);
        Task<JObject> GetDailyData(int whouse, UserProfile user);
        Task<JObject> GetExpensesAsync(int whouse, UserProfile user);
        Task<JObject> GetMonthData(int whouse, UserProfile user);
        Task<JObject> GetStocksAsync(int whouse, UserProfile user);
    }
}
