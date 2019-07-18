using System.Collections.Generic;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.Web.Models.ViewModel;

namespace SmartRetail.App.Web.Models.Interface
{
    public interface IShopSerivce
    {
        IEnumerable<ShopViewModel> GetStocks(UserProfile user);
    }
}