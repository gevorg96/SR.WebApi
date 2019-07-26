using SmartRetail.App.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace SmartRetail.App.DAL.Repository.Interfaces
{
    public interface IOrdersRepository
    {
        Task<Orders> GetByIdAsync(int orderId);
        Task<Orders> GetByIdWithMultiAsync(int orderId);
        Task<int> AddOrderAsync(Orders order);
        Task<int> AddCancellationAsync(Orders order);
        Task<IEnumerable<Orders>> GetOrdersByShopIdInDateRange(int shopId, DateTime from, DateTime to);
        Task<IEnumerable<Orders>> GetCancellationsByShopIdInDateRange(int shopId, DateTime from, DateTime to);
        Task<Orders> GetByShopIdOnDate(int shopId, DateTime reportDate, bool isOrder);
    }
}
