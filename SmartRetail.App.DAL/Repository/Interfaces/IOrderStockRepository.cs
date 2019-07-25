using SmartRetail.App.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartRetail.App.DAL.Repository.Interfaces
{
    public interface IOrderStockRepository
    {
        Task<IEnumerable<OrderStock>> GetOrderStocksByProdIds(IEnumerable<int> prodIds);
        Task<IEnumerable<OrderStock>> GetOrderStocksByProdId(int prodId);
        Task<IEnumerable<OrderStock>> GetPureOrderStocksByProdId(int prodId);
        Task<IEnumerable<OrderStock>> GetPureOrderStocksByProdAndShopIds(int prodId, int shopId);
        Task AddOrderStockAsync(OrderStock entity);
        Task UpdateOrderStockAsync(OrderStock entity);
    }
}
