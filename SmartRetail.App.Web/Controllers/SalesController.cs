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
        private readonly IShopSerivce shopSerivce;

        public SalesController(ISalesService service, IUserRepository userRepo, IShopSerivce shopSerivce)
        {
            _service = service;
            _userRepo = userRepo;
            this.shopSerivce = shopSerivce;
        }

        [HttpGet("/getshops")]
        public IEnumerable<ShopViewModel> GetStocks()
        {
            var user = _userRepo.GetByLogin(User.Identity.Name);
            var stocks = shopSerivce.GetStocks(user).OrderBy(p => p.id);
            return stocks;
        }

        //[HttpGet("/sales")]
        //public JsonResult GetSales(int shopId, DateTime from, DateTime to, bool orderByDesc = true)
        //{
        //    var user = _userRepo.GetByLogin(User.Identity.Name);
        //    var sales = _service.GetSales(user.UserId, shopId, from, to);
        //    if (sales == null)
        //        return Json("{ message: \"Нет данных...\" }");
            
            
        //    if (orderByDesc)
        //        return Json(sales.OrderByDescending(p => p.reportDate));
            
        //    return Json(sales.OrderBy(p => p.reportDate));
        //}
        
        [HttpGet]
        public IEnumerable<SalesViewModel> GetSales([FromBody] SalesRequestViewModel model)
        {
            var user = _userRepo.GetByLogin(User.Identity.Name);
            var sales = _service.GetSales(user.UserId, model.shopId ?? 0, model.from, model.to);
            return model.orderByDesc ? sales.OrderByDescending(p => p.reportDate) : sales.OrderBy(p => p.reportDate);
        }
    }
}