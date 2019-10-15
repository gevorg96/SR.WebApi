using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SmartRetail.App.Web.Controllers
{
    [Authorize]
    public class StocksPageController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View();
        }
    }
}