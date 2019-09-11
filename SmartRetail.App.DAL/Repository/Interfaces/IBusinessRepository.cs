using System.Threading.Tasks;
using SmartRetail.App.DAL.Entities;

namespace SmartRetail.App.DAL.Repository.Interfaces
{
    public interface IBusinessRepository
    {
        void Add(Business b);
        Business GetById(int businessId);
        Task<Business> GetByIdAsync(int businessId);
    }
}
