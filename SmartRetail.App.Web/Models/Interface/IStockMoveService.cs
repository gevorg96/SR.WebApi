using SmartRetail.App.DAL.Entities;
using SmartRetail.App.Web.Models.ViewModel.StockMove;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartRetail.App.Web.Models.Interface
{
    public interface IStockMoveService
    {
        IEnumerable<StockMoveViewModel> GetProducts(UserProfile user, int shopId, string name);
        Task MoveStocks(UserProfile user, StockMoveRequestViewModel model);
    }
}
