using System.Collections.Generic;
using System.Threading.Tasks;
using SmartRetail.App.DAL.Entities;

namespace SmartRetail.App.DAL.Repository
{
    public interface IUnitRepository
    {
        Task<IEnumerable<Units>> GetAllUnitsAsync();
    }
}