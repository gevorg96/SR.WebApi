using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SmartRetail.App.DAL.Repository;
using SmartRetail.App.Web.Models;
using SmartRetail.App.Web.Models.Interface;
using SmartRetail.App.Web.Models.Validation;
using SmartRetail.App.Web.Models.ViewModel;
using SmartRetail.App.Web.Models.ViewModel.Products;
using SmartRetail.App.Web.ViewModels;

namespace SmartRetail.App.Web.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("stocks")]
    [Authorize]
    [ApiController]
    public class StocksController : Controller
    {
        private readonly IStockService _service;
        private readonly IUserRepository _userRepo;
        
        public StocksController(IStockService service, IUserRepository userRepo)
        {
            _service = service;
            _userRepo = userRepo;
        }
        
        [HttpGet]
        public FilteredProductViewModel GetProducts([FromBody] ProductFilterViewModel model)
        {
            var user = _userRepo.GetByLogin(User.Identity.Name);
            var stocks = _service.GetStocks(user, model.shopId);
            if (stocks == null || !stocks.Any())
            {
                var vm = new FilteredProductViewModel
                {
                    Products = stocks, 
                    PageViewModel = new PageViewModel(1, 1, 0),
                };
                return vm;
            }
            if (!string.IsNullOrEmpty(model.name))
            {
                stocks = stocks.Where(p => p.ProdName.Contains(model.name));
            }
            if (!model.page.HasValue)
            {
                model.page = 1;
            }
            if (!model.limit.HasValue)
            {
                model.limit = 5;
            }

            var items = stocks.Skip((model.page.Value - 1) * model.limit.Value).Take(model.limit.Value).ToList();

            var prod = new FilteredProductViewModel
            {
                Products = items,
                PageViewModel = new PageViewModel(stocks.Count(), model.page.Value, model.limit.Value),
                SelectedProductName = model.name,
            };
            return prod;
        }
    }
}