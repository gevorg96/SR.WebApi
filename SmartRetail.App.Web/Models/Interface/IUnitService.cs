using SmartRetail.App.DAL.Entities;
using SmartRetail.App.Web.Models.ViewModel.Units;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartRetail.App.Web.Models.Interface
{
    public interface IUnitService
    {
        Task<IEnumerable<UnitViewModel>> GetUnitsAsync();
    }
}
