using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SmartRetail.App.DAL.Repository;
using SmartRetail.App.Web.Models;
using SmartRetail.App.Web.Models.Interface;
using SmartRetail.App.Web.Models.ViewModel;
using SmartRetail.App.Web.Models.ViewModel.Products;
using SmartRetail.App.Web.ViewModels;

namespace SmartRetail.App.Web.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("products")]
    [Authorize]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;
        private readonly IUserRepository _userRepo;
        public ProductsController(IProductService service, IUserRepository userRepo)
        {
            _service = service;
            _userRepo = userRepo;
        }

        [HttpGet]
        public async Task<FilteredProductViewModel> GetProducts([FromBody]ProductFilterViewModel model)
        {
            var user = _userRepo.GetByLogin(User.Identity.Name);
            var products = await _service.GetProducts(user);

            if (products == null || !products.Any())
            {
                return new FilteredProductViewModel { Products = new List<ProductViewModel>() };
            }

            if (!string.IsNullOrEmpty(model.name))
            {
                products = products.Where(p => p.ProdName.ToLower().StartsWith(model.name.ToLower()));
            }

            if (!string.IsNullOrEmpty(model.color))
            {
                products = products.Where(p => p != null && !string.IsNullOrEmpty(p.Color) && p.Color.ToLower().Contains(model.color.ToLower()));
            }

            if (!string.IsNullOrEmpty(model.size))
            {
                products = products.Where(p => p != null && !string.IsNullOrEmpty(p.Size) && p.Size.ToLower().Contains(model.size));
            }

            if (!model.page.HasValue)
            {
                model.page = 1;
            }
            if (!model.limit.HasValue)
            {
                model.limit = 5;
            }
            var items = products.Skip((model.page.Value - 1) * model.limit.Value).Take(model.limit.Value).ToList();

            var prod = new FilteredProductViewModel
            {
                Products = items,
                PageViewModel = new PageViewModel(products.Count(), model.page.Value, model.limit.Value),
                SelectedProductName = model.name,
                SelectedProductColor = model.color,
                SelectedProductSize = model.size
            };
            return prod;
        }

        [HttpGet("{id}")]
        public IActionResult GetProduct(int id)
        {
            if (id == 0)
            {
                return new BadRequestObjectResult("Не выбран продукт.");
            }
            var user = _userRepo.GetByLogin(User.Identity.Name);
            try
            {
                var product = _service.GetProduct(user, id);
                if (product != null)
                {
                    return Ok(product);
                }
                else
                {
                    throw new Exception("Нет такого товара.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] ProductDetailViewModel product)
        {
            var user = _userRepo.GetByLogin(User.Identity.Name);
            try
            {
                await _service.AddProduct(user, product);
                return Ok(product);
            }
            catch (Exception e)
            {
                throw new Exception("Возникла ошибка при добавлении: " + e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductDetailViewModel product)
        {
            product.Id = id;
            var user = _userRepo.GetByLogin(User.Identity.Name);
            try
            {
                await _service.UpdateProduct(user, product);
                return Ok("Товар изменён.");
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }



        //todo
        [HttpGet("/addproduct")]
        public async Task<ProductDetailRequestViewModel> GetInfoForCreateAsync()
        {
            var user = _userRepo.GetByLogin(User.Identity.Name);
            return await _service.GetChoiceForUserAsync(user);
        }

        

        [HttpPost("/getproductgroups")]
        public async Task<ProductGroupViewModel> GetProductGroups([FromBody]FolderRequestViewModel folderPath)
        {
            var user = _userRepo.GetByLogin(User.Identity.Name);

            return await _service.GetNexLevelGroup(user, folderPath.path, folderPath.needProducts);
        }

        [HttpPost("/searchproductgroups")]
        public async Task<ProductGroupViewModel> SearchProductGroups([FromBody] FolderRequestSearchVeiwModel model)
        {
            var user = _userRepo.GetByLogin(User.Identity.Name);
            var result = await _service.Search(user, model.searchCriteria, 0, 1000, model.path);
            var page = 1;
            var limit = 5;
            if (model.page != 0 && model.limit != 0)
            {
                page = model.page;
                limit = model.limit;
            }
            return result;
        }

    }
}