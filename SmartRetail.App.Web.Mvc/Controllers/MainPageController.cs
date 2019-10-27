using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SmartRetail.App.Web.Controllers
{
    [Authorize]
    public class MainPageController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View();
        }
    }
}