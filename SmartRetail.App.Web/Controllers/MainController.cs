using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SmartRetail.App.Web.Models.Interface;
using System.Threading.Tasks;
using SmartRetail.App.DAL.Repository;

namespace SmartRetail.App.Web.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class MainController : Controller
    {
        private readonly IInformationService _service;
        private readonly IShopSerivce shopService;
        private readonly IUserRepository userRepo;
        public MainController(IInformationService service, IShopSerivce _shopService, IUserRepository userRepository)
        {
            _service = service;
            shopService = _shopService;
            userRepo = userRepository;
        }

        [HttpGet("/getdailysales")]
        public async Task<JsonResult> GetCurrentDailySales(int? whouse)
        {
            var user = await userRepo.GetByLogin(User.Identity.Name);
            var shops = shopService.GetStocks(user);
            int shopId = shops.FirstOrDefault().id;

            if (whouse != null)
            {
                if (shops.Select(p => p.id).Contains(whouse.Value))
                {
                    shopId = whouse.Value;
                }
            }

            var daily = await _service.GetDailyData(shopId, user);
            Response.ContentType = "application/json";
            return Json(daily);
        }

        [HttpGet("/getmonthlysales")]
        public async Task<JsonResult> GetMonthlySales(int? whouse)
        {
            var user = await userRepo.GetByLogin(User.Identity.Name);
            var shops = shopService.GetStocks(user);
            int shopId = shops.FirstOrDefault().id;

            if (whouse != null)
            {
                if (shops.Select(p => p.id).Contains(whouse.Value))
                {
                    shopId = whouse.Value;
                }
            }

            var monthly = await _service.GetMonthData(shopId, user);
            Response.ContentType = "application/json";
            return Json(monthly);
        }

        [HttpGet("/getstocks")]
        public async Task<JsonResult> GetStocks(int? whouse)
        {
            var user = await userRepo.GetByLogin(User.Identity.Name);
            var shops = shopService.GetStocks(user);
            int shopId = shops.FirstOrDefault().id;

            if (whouse != null)
            {
                if (shops.Select(p => p.id).Contains(whouse.Value))
                {
                    shopId = whouse.Value;
                }
            }

            var stocks = await _service.GetStocksAsync(shopId, user);
            Response.ContentType = "application/json";
            return Json(stocks);
        }

        [HttpGet("/getexpenses")]
        public async Task<JsonResult> GetExpenses(int? whouse)
        {
            var user = await userRepo.GetByLogin(User.Identity.Name);
            var shops = shopService.GetStocks(user);
            int shopId = shops.FirstOrDefault().id;

            if (whouse != null)
            {
                if (shops.Select(p => p.id).Contains(whouse.Value))
                {
                    shopId = whouse.Value;
                }
            }

            var exp = await _service.GetExpensesAsync(shopId, user);
            Response.ContentType = "application/json";
            return Json(exp);
        }
    }
}