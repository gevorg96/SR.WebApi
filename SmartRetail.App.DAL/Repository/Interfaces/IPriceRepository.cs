using System.Threading.Tasks;
using SmartRetail.App.DAL.Entities;

namespace SmartRetail.App.DAL.Repository.Interfaces
{
    public interface IPriceRepository
    {
        Task<int> InsertUow(Price price);
        Task<bool> UpdateUow(Price price);

        void Add(Price entity);
        Price GetPriceByProdId(int prodId);
        void Update(Price entity);
    }
}