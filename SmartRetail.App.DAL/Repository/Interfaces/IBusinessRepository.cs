using System.Collections.Generic;
using SmartRetail.App.DAL.Entities;
using System.Threading.Tasks;

namespace SmartRetail.App.DAL.Repository
{
    public interface IBusinessRepository
    {
        IEnumerable<Business> GetAll();
        IEnumerable<Business> GetWithFilter(string field, string value);
        void Add(Business b);
        Business GetById(int businessId);
        Task<Business> GetByIdAsync(int businessId);
    }
}
