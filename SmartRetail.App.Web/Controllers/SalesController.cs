﻿using System;
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
using System.Net.Http;
using SmartRetail.App.Web.Models.Service;

namespace SmartRetail.App.Web.Controllers
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
        private HttpClient client;

        public SalesController(ISalesService _service, IUserRepository _userRepo, IShopSerivce _shopService)
        {
            service = _service;
            userRepo = _userRepo;
            shopService = _shopService;
        }

        [HttpGet]
        public async Task<IEnumerable<SalesViewModel>> GetSales(int? shopId, DateTime? from, DateTime? to, bool orderByDesc = true)
        {
            var user = userRepo.GetByLogin(User.Identity.Name);

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

        [HttpPost]
        public async Task<IActionResult> AddSale([FromBody] SalesCreateViewModel model)
        {
            var user = userRepo.GetByLogin(User.Identity.Name);
            var shops = shopService.GetStocks(user).Select(p => p.id);

            if (shops.Contains(model.shopId))
            {
                try
                {
                    await service.AddSale(model);
                    return Ok("Продажа добавлена.");
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