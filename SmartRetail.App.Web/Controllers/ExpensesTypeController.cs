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
    [Route("expensesType")]
    [Authorize]
    [ApiController]
    public class ExpensesTypeController : Controller
    {
        private readonly IExpensesTypeService service;
        public ExpensesTypeController(IExpensesTypeService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IEnumerable<ExpensesTypeViewModel>> GetUnitsAsync()
        {
            return await service.GetExpensesTypes();
        }
    }
}