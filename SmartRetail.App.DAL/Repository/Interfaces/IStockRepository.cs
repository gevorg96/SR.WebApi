using System.Collections.Generic;
using SmartRetail.App.DAL.Entities;

namespace SmartRetail.App.DAL.Repository
{
    public interface IStockRepository
    {
        IEnumerable<Stock> GetStocksWithProducts(int shopId);
        IEnumerable<Stock> GetStocksWithProductsByBusiness(int businessId);
        Stock GetStockByShopAndProdIds(int shopId, int prodId);
        int Add(Stock entity);
    }
}
