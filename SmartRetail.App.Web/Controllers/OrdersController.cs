using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SmartRetail.App.DAL.Repository;
using SmartRetail.App.Web.Models.Interface;
using SmartRetail.App.Web.Models.ViewModel.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartRetail.App.Web.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("orders")]
    [Authorize]
    [ApiController]
    public class OrdersController : Controller
    {
        private readonly IOrderService orderService;
        private readonly IUserRepository userRepository;
        private readonly IShopSerivce shopService;
        public OrdersController(IUserRepository userRepo, IOrderService service, IShopSerivce _shopService)
        {
            userRepository = userRepo;
            orderService = service;
            shopService = _shopService;
        }

        [HttpGet]
        public async Task<IEnumerable<OrderViewModel>> GetOrders(int? shopId, DateTime? from, DateTime? to, bool orderByDesc = true)
        {
            var user = userRepository.GetByLogin(User.Identity.Name);

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

            var orders = await orderService.GetOrders(user, from.Value, to.Value, shopId ?? 0);
            return orderByDesc ? orders.OrderByDescending(p => p.reportDate) : orders.OrderBy(p => p.reportDate);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            if (id == 0)
            {
                return new BadRequestObjectResult("Не выбран приход.");
            }
            var user = userRepository.GetByLogin(User.Identity.Name);
            try
            {
                var order = await orderService.GetOrder(user, id);
                if (order != null)
                {
                    return Ok(order);
                }
                else
                {
                    throw new Exception("Нет такого прихода.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddOrders([FromBody]OrderCreateViewModel model)
        {
            var user = userRepository.GetByLogin(User.Identity.Name);
            var shops = shopService.GetStocks(user).Select(p => p.id);

            if (shops.Contains(model.shopId))
            {
                try
                {
                    var order = await orderService.AddOrder(model);
                    return Ok(order);
                }
                catch (Exception ex)
                {
                    return new UnprocessableEntityResult();
                }
            }

            return BadRequest("Не выбран магазин/склад, либо выбран не тот :(");
        }
    }
}