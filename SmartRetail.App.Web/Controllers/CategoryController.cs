using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SmartRetail.App.DAL.Repository.Interfaces;
using SmartRetail.App.Web.Models.Interface;
using SmartRetail.App.Web.Models.ViewModel.Folders;
using SmartRetail.App.Web.Models.ViewModel.Products;

namespace SmartRetail.App.Web.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("categories")]
    [Authorize]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly IUserRepository userRepo;
        private readonly ICategoryService catService;

        public CategoryController(IUserRepository _userRepo, ICategoryService _catService)
        {
            userRepo = _userRepo;
            catService = _catService;
        }

        //[HttpGet]
        //public async Task<ProductGroupViewModel> GetProductGroups([FromBody]FolderRequestViewModel folderPath)
        //{
        //    var user = userRepo.GetByLogin(User.Identity.Name);

        //    return await catService.GetNexLevelGroup(user, folderPath.path, folderPath.needProducts);
        //}

        [HttpPost]
        public async Task<ProductGroupViewModel> Search([FromBody] FolderRequestSearchVeiwModel model)
        {
            var user = await userRepo.GetByLogin(User.Identity.Name);
            if (string.IsNullOrEmpty(model.searchCriteria))
            {
                return await catService.GetNexLevelGroup(user, model.path, model.needProducts);
            }
            return await catService.Search(user, model.searchCriteria, model.path);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTreeWithoutProducts()
        {
            var user = await userRepo.GetByLogin(User.Identity.Name);
            var tree = await catService.GetFullFolderTree(user);
            return Ok(tree);
        }
    }
}