using SmartRetail.App.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartRetail.App.DAL.Repository.Interfaces
{
    public interface ICostRepository
    {
        IEnumerable<Cost> GetByProdId(int prodId);
        Cost GetByProdAndShopIds(int prodId, int shopId);
        Task UpdateCostValueAsync(Cost entity);
        Task AddCostAsync(Cost entity);
    }
}
