using SmartRetail.App.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartRetail.App.DAL.Repository.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Orders>> GetOrdersByProdId(int prodId);
        Task<IEnumerable<Orders>> GetOrdersByProdIds(IEnumerable<int> prodIds);
        Task AddOrderAsync(Orders entity);
    }
}