using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SmartRetail.App.Web.Controllers
{
    [Authorize]
    public class ExpensesPageController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View();
        }
    }
}