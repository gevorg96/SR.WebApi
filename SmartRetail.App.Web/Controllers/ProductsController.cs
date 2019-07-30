using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
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
        public async Task<IActionResult> GetProducts(int? page = null, int? limit = null, string name = null, string color = null, string size = null)
        {
            var user = _userRepo.GetByLogin(User.Identity.Name);
            var products = await _service.GetProducts(user);

            if (products == null || !products.Any())
            {
                var m = new FilteredProductViewModel { Products = new List<ProductViewModel>() };
                return Ok(m);
            }

            if (!string.IsNullOrEmpty(name))
            {
                products = products.Where(p => p.ProdName.ToLower().StartsWith(name.ToLower()));
            }

            if (!string.IsNullOrEmpty(color))
            {
                products = products.Where(p => p != null && !string.IsNullOrEmpty(p.Color) && p.Color.ToLower().Contains(color.ToLower()));
            }

            if (!string.IsNullOrEmpty(size))
            {
                products = products.Where(p => p != null && !string.IsNullOrEmpty(p.Size) && p.Size.ToLower().Contains(size));
            }

            if (page == null || limit == null)
            {
                return Ok(products);
            }

            var items = products.Skip((page.Value - 1) * limit.Value).Take(limit.Value).ToList();

            var prod = new FilteredProductViewModel
            {
                Products = items,
                PageViewModel = new PageViewModel(products.Count(), page.Value, limit.Value),
                SelectedProductName = name,
                SelectedProductColor = color,
                SelectedProductSize = size
            };
            return Ok(prod);
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
        public async Task<IActionResult> AddProduct([FromForm] ProductDetailViewModel product)
        {
            var user = _userRepo.GetByLogin(User.Identity.Name);
            try
            {
                var newFileName = string.Empty;

                if (HttpContext.Request.Form.Files != null)
                {
                    var fileName = string.Empty;
                    string PathDB = string.Empty;

                    var file = product.img;
                    if (file.Length > 0)
                    {
                        //Getting FileName
                        fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

                        //Assigning Unique Filename (Guid)
                        var myUniqueFileName = Convert.ToString(Guid.NewGuid());

                        //Getting file Extension
                        var FileExtension = Path.GetExtension(fileName);

                        // concating  FileName + FileExtension
                        newFileName = myUniqueFileName + FileExtension;

                        product.ImgBase64 = newFileName;
                    }
                }
                var prod = await _service.AddProduct(user, product);
                return Ok(prod);

            }
            catch (Exception ex)
            {
                var prod = await _service.AddProduct(user, product);
                return Ok(prod);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductDetailViewModel product)
        {
            product.Id = id;
            var user = _userRepo.GetByLogin(User.Identity.Name);
            try
            {
                var newFileName = string.Empty;

                if (HttpContext.Request.Form.Files != null)
                {
                    var fileName = string.Empty;
                    string PathDB = string.Empty;

                    var file = product.img;
                    if (file.Length > 0)
                    {
                        //Getting FileName
                        fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

                        //Assigning Unique Filename (Guid)
                        var myUniqueFileName = Convert.ToString(Guid.NewGuid());

                        //Getting file Extension
                        var FileExtension = Path.GetExtension(fileName);

                        // concating  FileName + FileExtension
                        newFileName = myUniqueFileName + FileExtension;
                        product.ImgBase64 = newFileName;
                    }
                }
                var p = await _service.UpdateProduct(user, product);
                return Ok(p);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
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