using System;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReflectionIT.Mvc.Paging;
using SmartRetail.App.DAL.BLL.DataServices;
using SmartRetail.App.DAL.Repository.Interfaces;
using SmartRetail.App.Web.Models.Interface;
using SmartRetail.App.Web.Models.ViewModel.Folders;
using SmartRetail.App.Web.Models.ViewModel.Products;

namespace SmartRetail.App.Web.Controllers
{
    [Authorize]
    public class ProductsPageController : Controller
    {
        private readonly IProductService _service;
        private readonly IUserRepository _userRepo;
        private readonly IUnitService _unitService;
        private readonly ICategoryService _categoryService;
        private readonly IFoldersDataService _foldersDataService;
        public ProductsPageController(IProductService service, IUserRepository userRepo, IUnitService unitService, 
            ICategoryService categoryService, IFoldersDataService foldersDataService)
        {
            _service = service;
            _userRepo = userRepo;
            _unitService = unitService;
            _categoryService = categoryService;
            _foldersDataService = foldersDataService;
        }

        public async Task<IActionResult> Index(int? page = 1, string name = null)
        {
            int? limit = 10;
            var user = await _userRepo.GetByLogin(User.Identity.Name);
            if (user == null)
            {
                return Unauthorized();
            }
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

        
        
        
        [HttpGet]
        [Route("Groups")]
        public async Task<IActionResult> Groups()
        {
            var user = await _userRepo.GetByLogin(User.Identity.Name);
            if (user == null)
            {
                return Unauthorized();
            }
            var tree = await _categoryService.GetNexLevelGroup(user, null, true);
            return View(tree);
        }

        [HttpPost]
        [Route("Groups")]
        public async Task<IActionResult> Groups([FromBody]Fullpath fullpath)
        {
            var user = await _userRepo.GetByLogin(User.Identity.Name);
            if (user == null)
            {
                return Unauthorized();
            }
            var res = await _categoryService.GetNexLevelGroup(user, fullpath.fullpath, true);
            return Ok(res);
        }



        [HttpGet("{id}")]
        [Route("FoldersTree")]
        public async Task<IActionResult> Folders(int id)
        {
            var user = await _userRepo.GetByLogin(User.Identity.Name);
            if (user == null)
            {
                return Unauthorized();
            }
            var res =  await _foldersDataService.GetById(id, user.business_id.Value);
            return Ok(res);
        }
        
        [HttpGet]
        [Route("FoldersTree")]
        public async Task<IActionResult> Folders()
        {
            var user = await _userRepo.GetByLogin(User.Identity.Name);
            if (user == null)
            {
                return Unauthorized();
            }
            var res =  await _categoryService.GetNexLevelGroup(user, null, false);
            return PartialView(res);
        }
        
        [HttpPost]
        [Route("FoldersTree")]
        public async Task<IActionResult> Folders([FromBody]Fullpath fullpath)
        {
            var user = await _userRepo.GetByLogin(User.Identity.Name);
            if (user == null)
            {
                return Unauthorized();
            }
            var res =   await _categoryService.GetNexLevelGroup(user, fullpath.fullpath, false);
            return Ok(res);
        }

        
        
        
        
        [HttpGet]
        [Route("FolderRename")]
        public async Task<IActionResult> ChangeFolderName(string path)
        {
            var user = await _userRepo.GetByLogin(User.Identity.Name);
            if (user == null)
            {
                return Unauthorized();
            }
            return PartialView(new Fullpath{fullpath = path});
        }
        
        [HttpPut]
        [Route("FolderRename")]
        public async Task<IActionResult> ChangeFolderName([FromBody]FolderRenameViewModel model)
        {
            try
            {
                var user = await _userRepo.GetByLogin(User.Identity.Name);
                if (user == null)
                {
                    return Unauthorized();
                }
                await _foldersDataService.RenameFolderByPath(model.PathToFolder, model.NewFolderName,
                    user.business_id.Value);
                return Ok(new FolderViewModel());
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
            
        }
        
        
        
        
        
        [HttpGet("{id}")]
        [Route("Edit")]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userRepo.GetByLogin(User.Identity.Name);
            if (user == null)
            {
                return Unauthorized();
            }
            var product = await _service.GetProduct(user, id);
            if (product.CategoryId.HasValue)
            {
                product.Category = await _foldersDataService.GetById(product.CategoryId.Value, user.business_id.Value);
            }
            ViewData["Units"] = await _unitService.GetUnitsAsync();
            ViewData["isEdit"] = id != 0;
            return PartialView(product);
        }

        [HttpPost]
        [Route("Edit")]
        public async Task<IActionResult> Edit()
        {
            var user = await _userRepo.GetByLogin(User.Identity.Name);
            if (user == null)
            {
                return Unauthorized();
            }
            return PartialView(null);
        }
        
        [HttpPut]
        public async Task<IActionResult> Edit([FromForm] ProductDetailViewModel product)
        {
            var user = await _userRepo.GetByLogin(User.Identity.Name);
            if (user != null)
            {
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
                    return NotFound();
                }
            }
            else
            {
                return Unauthorized();
            }
        }
    }

    public class Fullpath
    {
        public string fullpath { get; set; }
    }
}