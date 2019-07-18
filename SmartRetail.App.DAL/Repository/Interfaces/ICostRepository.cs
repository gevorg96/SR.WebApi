using SmartRetail.App.DAL.Entities;
using System.Collections.Generic;

namespace SmartRetail.App.DAL.Repository.Interfaces
{
    public interface ICostRepository
    {
        IEnumerable<Cost> GetByProdId(int prodId);
        Cost GetByProdAndShopIds(int prodId, int shopId);
    }
}
