using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SmartRetail.App.DAL.Repository;
using SmartRetail.App.Web.Models.Interface;
using SmartRetail.App.Web.Models.ViewModel.Orders;

namespace SmartRetail.App.Web.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("cancellations")]
    [Authorize]
    [ApiController]
    public class CancellationsController : Controller
    {
        private readonly ICancellationService cancellationService;
        private readonly IUserRepository userRepository;
        private readonly IShopSerivce shopService;
        public CancellationsController(IUserRepository userRepo, ICancellationService service, IShopSerivce _shopService)
        {
            userRepository = userRepo;
            cancellationService = service;
            shopService = _shopService;
        }

        [HttpGet]
        public async Task<IEnumerable<OrderViewModel>> GetCancellations(int? shopId, DateTime? from, DateTime? to, bool orderByDesc = true)
        {
            var user = await userRepository.GetByLogin(User.Identity.Name);

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

            var orders = await cancellationService.GetCancellations(user, from.Value, to.Value, shopId ?? 0);
            return orderByDesc ? orders.OrderByDescending(p => p.reportDate) : orders.OrderBy(p => p.reportDate);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCancellation(int id)
        {
            if (id == 0)
            {
                return new BadRequestObjectResult("Не выбрано списание.");
            }
            var user = await userRepository.GetByLogin(User.Identity.Name);
            try
            {
                var cancel = await cancellationService.GetCancellation(user, id);
                if (cancel != null)
                {
                    return Ok(cancel);
                }
                else
                {
                    throw new Exception("Нет такого списания.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddCancellation([FromBody]OrderCreateViewModel model)
        {
            var user = await userRepository.GetByLogin(User.Identity.Name);
            var shops = shopService.GetStocks(user).Select(p => p.id);

            if (shops.Contains(model.shopId))
            {
                try
                {
                    var cancel = await cancellationService.AddCancellations(model);
                    return Ok(cancel);
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
