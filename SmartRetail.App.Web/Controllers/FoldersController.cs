using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SmartRetail.App.DAL.BLL.DataServices;
using SmartRetail.App.DAL.BLL.HelperClasses;
using SmartRetail.App.DAL.Helpers;
using SmartRetail.App.DAL.Repository;
using SmartRetail.App.Web.Models.Interface;
using SmartRetail.App.Web.Models.ViewModel;
using SmartRetail.App.Web.Models.ViewModel.Folders;

namespace SmartRetail.App.Web.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("folders")]
    [Authorize]
    [ApiController]
    public class FoldersController : Controller
    {
        private readonly IFoldersDataService foldersDataService;
        private readonly IUserRepository userRepo;
        private readonly ICategoryService categoryService;

        public FoldersController(IFoldersDataService _foldersDataService, IUserRepository _userRepo, ICategoryService _categoryService)
        {
            foldersDataService = _foldersDataService;
            userRepo = _userRepo;
            categoryService = _categoryService;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ProductGroupViewModel))]
        public async Task<ActionResult<ProductGroupViewModel>> GetFolders(string folder = null)
        {
            var user = userRepo.GetByLogin(User.Identity.Name);
            return await categoryService.GetNexLevelGroup(user, folder, false);
        }

        [HttpPost]
        [ProducesResponseType(201)] //, Type = typeof(ProductDetailViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AddFolder([FromBody] FolderAddViewModel model)
        {
            var user = userRepo.GetByLogin(User.Identity.Name);
            if (string.IsNullOrEmpty(model.folderName))
            {
                return BadRequest("Отсутствует название новой папки.");
            }
            await foldersDataService.AddFoldersByPath(model.pathToAdd + "/" + model.folderName, user.business_id.Value);

        }

    }
}