using SmartRetail.App.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace SmartRetail.App.DAL.Repository.Interfaces
{
    public interface IOrdersRepository
    {
        Task<int> InsertUow(Order order);
        Task<Order> GetByIdWithMultiUow(int orderId);


        Task<Order> GetByIdAsync(int orderId);
        Task<Order> GetByIdWithMultiAsync(int orderId);
        Task<int> AddOrderAsync(Order order);
        Task<int> AddCancellationAsync(Order order);
        Task<IEnumerable<Order>> GetOrdersByShopIdInDateRange(int shopId, DateTime from, DateTime to);
        Task<IEnumerable<Order>> GetCancellationsByShopIdInDateRange(int shopId, DateTime from, DateTime to);
        Task<Order> GetByShopIdOnDate(int shopId, DateTime reportDate, bool isOrder);
    }
}
