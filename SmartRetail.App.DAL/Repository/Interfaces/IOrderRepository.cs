using SmartRetail.App.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartRetail.App.DAL.Repository.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Orders>> GetOrdersByProdId(int prodId);
        Task<IEnumerable<Orders>> GetOrdersByProdIds(IEnumerable<int> prodIds);
        Task<IEnumerable<Orders>> GetOrdersByShopId(int shopId, DateTime from, DateTime to);
        Task<Orders> GetLastOrderAsync(int shopId, int prodId, DateTime from, DateTime to);
        Task<IEnumerable<Orders>> GetCancellationsByShopId(int shopId, DateTime from, DateTime to);
        Task AddOrderAsync(Orders entity);
    }
}