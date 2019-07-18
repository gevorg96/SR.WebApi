using System.Collections.Generic;
using SmartRetail.App.DAL.Entities;

namespace SmartRetail.App.DAL.Repository
{
    public interface IPriceRepository
    {
        void Add(Price entity);
        IEnumerable<Price> GetPricesByValue(string field, string value);
        IEnumerable<Price> GetPricesByIds(IEnumerable<int> ids);
        Price GetPriceByProdId(int prodId);
        Price GetPriceByProdAndShopIds(int prodId, int shopId);
        void Update(Price entity);

    }
}