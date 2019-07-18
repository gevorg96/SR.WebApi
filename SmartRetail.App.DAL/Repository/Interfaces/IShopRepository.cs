using System.Collections.Generic;
using SmartRetail.App.DAL.Entities;

namespace SmartRetail.App.DAL.Repository
{
    public interface IShopRepository
    {
        IEnumerable<Shop> GetShopsByBusiness(int businessId);
        IEnumerable<Shop> GetShopsByBusinessMultiMappingProducts(int businessId);
        Shop GetShopMultiMappingProducts(int shopId);
        Shop GetById(int shopId);

    }
}
