using System.Collections.Generic;
using System.Threading.Tasks;
using SmartRetail.App.DAL.Entities;

namespace SmartRetail.App.DAL.Repository
{
    public interface IShopRepository
    {
        IEnumerable<Shop> GetWithFilter(string field, string value);
        IEnumerable<Shop> GetShopsByBusiness(int businessId);
        IEnumerable<Shop> GetShopsByBusinessMultiMappingProducts(int businessId);
        Shop GetShopMultiMappingProducts(int shopId);
        Shop GetById(int shopId);
        Task AddAsync(Shop entity);
        Task UpdateAsync(Shop entity);
    }
}
