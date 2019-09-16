using SmartRetail.App.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartRetail.App.DAL.Repository.Interfaces
{
    public interface IOrderStockRepository
    {
        Task<int> InsertUow(OrderStock orderStock);
        Task<IEnumerable<OrderStock>> GetPureOrderStocksByProdIdUow(int prodId);
        Task<IEnumerable<OrderStock>> GetPureOrderStocksByProdAndShopIdsUow(int prodId, int shopId);

        Task<IEnumerable<OrderStock>> GetPureOrderStocksByProdId(int prodId);
        Task<IEnumerable<OrderStock>> GetPureOrderStocksByProdAndShopIds(int prodId, int shopId);
        Task AddOrderStockAsync(OrderStock entity);
        Task UpdateOrderStockAsync(OrderStock entity);
    }
}
