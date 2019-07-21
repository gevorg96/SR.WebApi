using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SmartRetail.App.DAL.Repository;
using SmartRetail.App.Web.Models.Interface;
using SmartRetail.App.Web.Models.ViewModel;
using SmartRetail.App.Web.Models.ViewModel.Expenses;
using SmartRetail.App.Web.ViewModels;

namespace SmartRetail.App.Web.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("expenses")]
    [Authorize]
    [ApiController]
    public class ExpensesController: ControllerBase
    {
        private readonly IExpensesService _service;
        private readonly IUserRepository _userRepo;
        public ExpensesController(IUserRepository userRepo, IExpensesService service)
        {
            _userRepo = userRepo;
            _service = service;
        }
        
        [HttpGet]
        public IEnumerable<ExpensesViewModel> GetExpenses(int? shopId, DateTime from, DateTime to)
        {
            var user = _userRepo.GetByLogin(User.Identity.Name);
            var expenses = _service.GetExpenses(user, shopId, from, to);
            if (expenses == null || !expenses.Any())
                return new List<ExpensesViewModel>();
            return expenses;
        }
    }
}