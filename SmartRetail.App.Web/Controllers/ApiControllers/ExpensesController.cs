using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SmartRetail.App.DAL.Repository.Interfaces;
using SmartRetail.App.Web.Models.Interface;
using SmartRetail.App.Web.Models.ViewModel.Expenses;

namespace SmartRetail.App.Web.Controllers.ApiControllers
{
    [EnableCors("MyPolicy")]
    [Route("expenses")]
    [Authorize]
    [ApiController]
    public class ExpensesController: ControllerBase
    {
        private readonly IExpensesService _service;
        private readonly IShopSerivce shopService;
        private readonly IUserRepository _userRepo;
        public ExpensesController(IUserRepository userRepo, IExpensesService service, IShopSerivce _shopService)
        {
            _userRepo = userRepo;
            _service = service;
            shopService = _shopService;
        }

        [HttpGet]
        public async Task<IEnumerable<ExpensesViewModel>> GetExpenses(int? shopId, DateTime from, DateTime to)
        {
            var user = await _userRepo.GetByLogin(User.Identity.Name);
            var expenses = await _service.GetExpenses(user, shopId, from, to);
            if (expenses == null || !expenses.Any())
                return new List<ExpensesViewModel>();
            return expenses;
        }

        [HttpPost]
        public async Task<IActionResult> AddExpenses([FromBody] ExpensesRequestViewModel model)
        {
            var user = await _userRepo.GetByLogin(User.Identity.Name);
            var shops = shopService.GetStocks(user).Select(p => p.id).ToList();
            if (shops.Contains(model.shopId.Value))
            {
                try
                {
                    var exModel = await _service.AddExpenses(user, model);
                    return Ok(exModel);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return BadRequest("Не выбран магазин/склад, либо выбран не тот.");
        }
    }
}