using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SmartRetail.App.DAL.Repository.Interfaces;
using SmartRetail.App.Web.Models.Interface;
using SmartRetail.App.Web.Models.ViewModel.Sales;

namespace SmartRetail.App.Web.Controllers.ApiControllers
{
    [EnableCors("MyPolicy")]
    [Route("sales")]
    [Authorize]
    [ApiController]
    public class SalesController : Controller
    {
        private readonly ISalesService service;
        private readonly IShopSerivce shopService;
        private readonly IUserRepository userRepo;

        public SalesController(ISalesService _service, IUserRepository _userRepo, IShopSerivce _shopService)
        {
            service = _service;
            userRepo = _userRepo;
            shopService = _shopService;
        }

        [HttpGet]
        public async Task<IEnumerable<SalesViewModel>> GetSales(int? shopId, DateTime? from, DateTime? to, bool orderByDesc = true)
        {
            var user = await userRepo.GetByLogin(User.Identity.Name);

            if (shopId == null)
            {
                var shops = shopService.GetStocks(user);
                shopId = shops.FirstOrDefault()?.id;
            }
            if (!from.HasValue || !to.HasValue)
            {
                from = DateTime.Now.AddMonths(-1);
                to = DateTime.Now;
            }

            var sales = await service.GetSales(user.UserId, shopId ?? 0, from.Value, to.Value);
            return orderByDesc ? sales.OrderByDescending(p => p.reportDate) : sales.OrderBy(p => p.reportDate);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBill(int id)
        {
            if (id == 0)
            {
                return new BadRequestObjectResult("Не выбрана продажа.");
            }
            var user = await userRepo.GetByLogin(User.Identity.Name);
            try
            {
                var product = await service.GetBill(user, id);
                if (product != null)
                {
                    return Ok(product);
                }
                else
                {
                    throw new Exception("Нет такой продажи.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddSale([FromBody] SalesCreateViewModel model)
        {
            var user = await userRepo.GetByLogin(User.Identity.Name);
            var shops = shopService.GetStocks(user).Select(p => p.id);

            if (shops.Contains(model.shopId))
            {
                try
                {
                    var billId = await service.AddSaleTransaction(model);
                    model.id = billId;
                    return Ok(model);
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