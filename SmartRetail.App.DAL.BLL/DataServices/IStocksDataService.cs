using System.Threading.Tasks;
using SmartRetail.App.DAL.BLL.HelperClasses;

namespace SmartRetail.App.DAL.BLL.DataServices
{
    public interface IStocksDataService
    {
        Task<SummaryStocks> GetStocks(int shopId);
    }
}
