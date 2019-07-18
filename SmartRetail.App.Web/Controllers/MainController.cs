using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SmartRetail.App.Web.Models.Interface;
using System.Threading.Tasks;

namespace SmartRetail.App.Web.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class MainController : Controller
    {
        private readonly IInformationService _service;

        public MainController(IInformationService service)
        {
            _service = service;
        }

        [HttpGet("/getdailysales")]
        public async Task<JsonResult> GetCurrentDailySales(int whouse)
        {
            var daily = await _service.GetDailyData(whouse);
            Response.ContentType = "application/json";
            return Json(daily);
        }

        [HttpGet("/getmonthlysales")]
        public async Task<JsonResult> GetMonthlySales(int whouse)
        {
            var monthly = await _service.GetMonthData(whouse);
            Response.ContentType = "application/json";
            return Json(monthly);
        }

        [HttpGet("/getstocks")]
        public JsonResult GetStocks(int whouse)
        {
            var stocks = _service.GetStocks(whouse);
            Response.ContentType = "application/json";
            return Json(stocks);
        }

        [HttpGet("/getexpenses")]
        public JsonResult GetExpenses(int whouse)
        {
            var exp = _service.GetExpenses(whouse);
            Response.ContentType = "application/json";
            return Json(exp);
        }

        [HttpGet("/getwarehouses")]
        public JsonResult GetWarehouses()
        {
            var wHouses = _service.GetWareHouses();
            Response.ContentType = "application/json";
            return Json(wHouses);
        }
    }
}