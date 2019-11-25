using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SmartRetail.App.Web.Models.Interface;
using SmartRetail.App.Web.Models.ViewModel.ExpensesType;

namespace SmartRetail.App.Web.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("api/expensesType")]
    [Authorize]
    [ApiController]
    public class ExpensesTypeController : Controller
    {
        private readonly IExpensesTypeService service;
        public ExpensesTypeController(IExpensesTypeService _service)
        {
            service = _service;
        }

        [HttpGet]
        public async Task<IEnumerable<ExpensesTypeViewModel>> GetExpensesType()
        {
            return await service.GetExpensesTypes();
        }
    }
}
