using System.Collections.Generic;
using System.Threading.Tasks;
using SmartRetail.App.DAL.Entities;

namespace SmartRetail.App.DAL.Repository.Interfaces
{
    public interface IUnitRepository
    {
        Task<IEnumerable<Unit>> GetAllUnitsAsync();
    }
}