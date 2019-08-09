using System.Collections.Generic;
using System.Threading.Tasks;
using SmartRetail.App.DAL.Entities;

namespace SmartRetail.App.Web.Models.Interface
{
    public interface IStockService
    {
        Task<IEnumerable<ProductViewModel>> GetStocks(UserProfile user, int? shopId);
    }
}