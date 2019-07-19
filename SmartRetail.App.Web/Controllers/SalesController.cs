using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SmartRetail.App.DAL.Repository;
using SmartRetail.App.Web.Models.Interface;
using SmartRetail.App.Web.Models.ViewModel;
using SmartRetail.App.Web.Models.ViewModel.Sales;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace SmartRetail.App.Web.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("sales")]
    [Authorize]
    [ApiController]
    public class SalesController : Controller
    {
        private readonly ISalesService _service;
        private readonly IUserRepository _userRepo;

        public SalesController(ISalesService service, IUserRepository userRepo)
        {
            _service = service;
            _userRepo = userRepo;
        }

        [HttpGet]
        public async Task<IEnumerable<SalesViewModel>> GetSales([FromBody] SalesRequestViewModel model)
        {
            var user = _userRepo.GetByLogin(User.Identity.Name);
            var sales = await _service.GetSales(user.UserId, model.shopId ?? 0, model.from, model.to);
            return model.orderByDesc ? sales.OrderByDescending(p => p.reportDate) : sales.OrderBy(p => p.reportDate);
        }
    }
}