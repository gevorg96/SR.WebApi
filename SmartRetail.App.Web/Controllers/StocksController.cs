using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SmartRetail.App.DAL.Repository.Interfaces;
using SmartRetail.App.Web.Models.Interface;
using SmartRetail.App.Web.Models.ViewModel;
using SmartRetail.App.Web.Models.ViewModel.Products;

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
        private readonly IShopSerivce _shopService;

        public StocksController(IStockService service, IUserRepository userRepo, IShopSerivce shopSerivce)
        {
            _service = service;
            _userRepo = userRepo;
            _shopService = shopSerivce;
        }
        
        [HttpGet]
        public async Task<FilteredProductViewModel> GetProducts(int? page = 1, int? limit = 10, string name = null, string color = null, string size = null, int? shopId = null)
        {
            var user = await _userRepo.GetByLogin(User.Identity.Name);

            if (shopId == null)
            {
                var shops = _shopService.GetStocks(user);
                shopId = shops.FirstOrDefault()?.id;
            }
            var stocks = await _service.GetStocks(user, shopId);
            if (stocks == null || !stocks.Any())
            {
                var vm = new FilteredProductViewModel
                {
                    Products = stocks, 
                    PageViewModel = new PageViewModel(1, 1, 0),
                };
                return vm;
            }
            if (!string.IsNullOrEmpty(name))
            {
                stocks = stocks.Where(p => p.ProdName.Contains(name));
            }

            var items = stocks.Skip((page.Value - 1) * limit.Value).Take(limit.Value).ToList();

            var prod = new FilteredProductViewModel
            {
                Products = items,
                PageViewModel = new PageViewModel(stocks.Count(), page.Value, limit.Value),
                SelectedProductName = name,
            };
            return prod;
        }
    }
}