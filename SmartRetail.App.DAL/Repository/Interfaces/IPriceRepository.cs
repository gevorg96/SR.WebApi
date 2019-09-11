using SmartRetail.App.DAL.Entities;

namespace SmartRetail.App.DAL.Repository.Interfaces
{
    public interface IPriceRepository
    {
        void Add(Price entity);
        Price GetPriceByProdId(int prodId);
        void Update(Price entity);
    }
}