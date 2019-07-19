using SmartRetail.App.DAL.Entities;
using System.Threading.Tasks;

namespace SmartRetail.App.DAL.Repository.Interfaces
{
    public interface ISupplierRepository
    {
        Task<Supplier> GetByIdAsync(int id);
        Task AddSupplierAsync(Supplier entity); 
    }
}
