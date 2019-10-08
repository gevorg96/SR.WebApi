using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SmartRetail.App.DAL.Repository.Interfaces;
using SmartRetail.App.Web.Models.Interface;
using SmartRetail.App.Web.Models.ViewModel;
using SmartRetail.App.Web.Models.ViewModel.Products;

namespace SmartRetail.App.Web.Controllers.ApiControllers
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
        [ProducesResponseType(200, Type = typeof(List<ProductViewModel>))]
        public async Task<ActionResult<List<ProductViewModel>>> GetProducts(int? page = null, int? limit = null, string name = null, string color = null, string size = null)
        {
            var user = await _userRepo.GetByLogin(User.Identity.Name);
            var products = await _service.GetProducts(user);

            if (products == null || !products.Any())
            {
                var m = new FilteredProductViewModel { Products = new List<ProductViewModel>() };
                return Ok(m);
            }

            // переделать на регулярное выражение
            string pattern = "\\b" + name + "\\S*";    //шаблон, по которому ищутся слова в строке
            var regex = new Regex(pattern);

            if (!string.IsNullOrEmpty(name))
            {
                products = products.Where(p => !string.IsNullOrEmpty(p.ProdName) && StartsWithAny(p.ProdName.ToLower(), name.ToLower()));
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
        [ProducesResponseType(200, Type = typeof(ProductViewModel))]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ProductViewModel>> GetProduct(int id)
        {
            if (id == 0)
            {
                return new BadRequestObjectResult("Не выбран продукт.");
            }
            var user = await _userRepo.GetByLogin(User.Identity.Name);
            try
            {
                var product = await _service.GetProduct(user, id);
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
        [ProducesResponseType(201, Type = typeof(ProductDetailViewModel))]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ProductDetailViewModel>> AddProduct([FromForm] ProductDetailViewModel product)
        {
            var user = await _userRepo.GetByLogin(User.Identity.Name);
            try
            {
                var newFileName = string.Empty;

                if (HttpContext.Request.Form.Files != null && HttpContext.Request.Form.Files.Any())
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
                var prod = await _service.AddProductTransaction(user, product);
                return Ok(prod);

            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ProductViewModel>> UpdateProduct(int id, [FromForm] ProductDetailViewModel product)
        {
            product.Id = id;
            var user = await _userRepo.GetByLogin(User.Identity.Name);
            try
            {
                var newFileName = string.Empty;

                if (HttpContext.Request.Form.Files != null && HttpContext.Request.Form.Files.Any())
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
                var p = await _service.UpdateProductTransaction(user, product);
                return Ok(p);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        private bool StartsWithAny(string prodName, string search)
        {
            var words = prodName.Split(' ');
            foreach (var word in words)
            {
                if (word.StartsWith(search))
                {
                    return true;
                }
            }

            return false;
        }

    }
}