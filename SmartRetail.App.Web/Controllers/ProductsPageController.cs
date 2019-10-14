using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReflectionIT.Mvc.Paging;
using SmartRetail.App.DAL.Repository.Interfaces;
using SmartRetail.App.Web.Models.Interface;
using SmartRetail.App.Web.Models.ViewModel.Products;

namespace SmartRetail.App.Web.Controllers
{
    [Authorize]
    public class ProductsPageController : Controller
    {
        private readonly IProductService _service;
        private readonly IUserRepository _userRepo;

        public ProductsPageController(IProductService service, IUserRepository userRepo)
        {
            _service = service;
            _userRepo = userRepo;
        }

        public async Task<IActionResult> Index(int? page = 1, string name = null)
        {
            int? limit = 10;
            var user = await  _userRepo.GetByLogin(User.Identity.Name);

            var products = await _service.GetProducts(user);

            if (products == null || !products.Any())
            {
                var emptyList = PagingList.Create<ProductViewModel>(products, limit.Value, page.Value);
                return View(emptyList);
            }
            if (!string.IsNullOrEmpty(name))
            {
                products = products.Where(p => !string.IsNullOrEmpty(p.ProdName) && ProductsController.StartsWithAny(p.ProdName.ToLower(), name.ToLower()));
            }

            var items = PagingList.Create<ProductViewModel>(products, limit.Value, page.Value);

            return View(items);
        }

    }
}