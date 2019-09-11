using System.Collections.Generic;
using System.Threading.Tasks;
using SmartRetail.App.DAL.Entities;

namespace SmartRetail.App.DAL.Repository.Interfaces
{
    public interface IShopRepository
    {
        IEnumerable<Shop> GetShopsByBusiness(int businessId);
        Shop GetById(int shopId);
        Task AddAsync(Shop entity);
        Task UpdateAsync(Shop entity);
    }
}
