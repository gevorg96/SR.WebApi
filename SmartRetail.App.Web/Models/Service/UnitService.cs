using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Repository;
using SmartRetail.App.Web.Models.Interface;
using SmartRetail.App.Web.Models.ViewModel.Units;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartRetail.App.Web.Models.Service
{
    public class UnitService : IUnitService
    {
        private readonly IUnitRepository unitRepo;
        public UnitService(IUnitRepository _unitRepo)
        {
            unitRepo = _unitRepo;
        }

        public async Task<IEnumerable<UnitViewModel>> GetUnitsAsync()
        {
            var units = await unitRepo.GetAllUnitsAsync();
            return units.Select(p => new UnitViewModel { id = p.id, name = p.value });
        }
    }
}
