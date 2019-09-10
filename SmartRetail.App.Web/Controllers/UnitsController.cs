using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SmartRetail.App.DAL.Repository;
using SmartRetail.App.Web.Models.Interface;
using SmartRetail.App.Web.Models.ViewModel.Units;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartRetail.App.Web.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("units")]
    [Authorize]
    [ApiController]
    public class UnitsController : Controller
    {
        private static int counter = 0;

        private readonly IUnitService service;
        private readonly IUserRepository userRepo;
        public UnitsController(IUserRepository _userRepo, IUnitService service)
        {
            this.service = service;
            userRepo = _userRepo;
        }

        [HttpGet]
        public async Task<IEnumerable<UnitViewModel>> GetUnitsAsync()
        {
            counter++;
            return await service.GetUnitsAsync();
        }
    }
}