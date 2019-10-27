using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SmartRetail.App.Web.Controllers
{
    [Authorize]
    public class SalesPageController: Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}