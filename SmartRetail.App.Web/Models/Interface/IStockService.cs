using System.Collections.Generic;
using System.Threading.Tasks;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.Web.Models.ViewModel.Products;

namespace SmartRetail.App.Web.Models.Interface
{
    public interface IStockService
    {
        Task<IEnumerable<ProductViewModel>> GetStocks(UserProfile user, int? shopId);
    }
}