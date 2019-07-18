using System.Collections.Generic;
using SmartRetail.App.DAL.Entities;

namespace SmartRetail.App.Web.Models.Interface
{
    public interface IStockService
    {
        IEnumerable<ProductViewModel> GetStocks(UserProfile user, int? shopId);
    }
}