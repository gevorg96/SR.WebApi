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
using SmartRetail.App.DAL.Repository.Interfaces;
using SmartRetail.App.Web.Models.Interface;
using SmartRetail.App.Web.Models.ViewModel;
using SmartRetail.App.Web.Models.ViewModel.Folders;
using SmartRetail.App.Web.Models.ViewModel.Products;

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
            var user = await userRepo.GetByLogin(User.Identity.Name);
            ProductGroupViewModel grouplevel;

            if (user == null)
            {
                return new UnauthorizedResult();
            }

            try
            {
                grouplevel = await categoryService.GetNexLevelGroup(user, folder, false);
            }
            catch (Exception)
            {
                return new ConflictObjectResult("Что-то пошло не так...");
            }

            return Ok(grouplevel);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(FolderViewModel))]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await userRepo.GetByLogin(User.Identity.Name);
            if (user == null)
            {
                return new UnauthorizedResult();
            }

            ImgTwinModel folder = null;

            try
            {
                folder = await foldersDataService.GetById(id, user.business_id.Value);
            }
            catch (Exception)
            {
                return new ConflictObjectResult("Что-то пошло не так...");
            }
            if (folder == null)
            {
                return new EmptyResult();
            }

            return Ok(new FolderViewModel
            {
                id = folder.id,
                folder = folder.folder,
                fullpath = folder.fullpath
            });
        }

        [HttpPost]
        [ProducesResponseType(201)] 
        [ProducesResponseType(400)]
        public async Task<IActionResult> AddFolder([FromBody] FolderAddViewModel model)
        {
            var user = await userRepo.GetByLogin(User.Identity.Name);
            if (user == null)
            {
                return new UnauthorizedResult();
            }
            if (string.IsNullOrEmpty(model.folderName) || string.IsNullOrEmpty(model.pathToAdd))
            {
                return BadRequest("Отсутствует название новой папки или путь до папки.");
            }

            try
            {
                await foldersDataService.AddFoldersByPath(model.pathToAdd + "/" + model.folderName, user.business_id.Value);
            }
            catch (Exception)
            {
                return new ConflictObjectResult("Что-то пошло не так...");
            }
            return Ok("Папка '" + model.folderName +"' создана." );
        }


        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> RenameFolder([FromBody] FolderRenameViewModel model)
        {
            var user = await userRepo.GetByLogin(User.Identity.Name);
            if (user == null)
            {
                return new UnauthorizedResult();
            }
            if (string.IsNullOrEmpty(model.NewFolderName) || string.IsNullOrEmpty(model.PathToFolder))
            {
                return BadRequest("Отсутствует новое название папки или путь до папки.");
            }

            try
            {
                await foldersDataService.RenameFolderByPath(model.PathToFolder, model.NewFolderName, user.business_id.Value);
            }
            catch (Exception)
            {
                return new ConflictObjectResult("Что-то пошло не так...");
            }
            return Ok("Папка '" + model.PathToFolder.Split('/').Last() + "' изменена на '" +model.NewFolderName +"'.");
        }

        [HttpPatch]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ReplaceFolder([FromBody] FolderReplaceViewModel model)
        {
            var user = await userRepo.GetByLogin(User.Identity.Name);
            if (user == null)
            {
                return new UnauthorizedResult();
            }
            if (string.IsNullOrEmpty(model.newPath) || string.IsNullOrEmpty(model.pathToFolder))
            {
                return BadRequest("Отсутствует путь до папки или новый путь.");
            }

            try
            {
                await foldersDataService.ReplaceFolderByPath(model.pathToFolder, model.newPath, user.business_id.Value, model.Copy);
            }
            catch (Exception)
            {
                return new ConflictObjectResult("Что-то пошло не так...");
            }
            return Ok("Папка '" + model.pathToFolder.Split('/').Last() + (!model.Copy ? "' перемещена в папку '" : "' скопирована в папку '" )+ model.newPath.Split('/').Last() + "'.");
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteFolder(int id)
        {
            var user = await userRepo.GetByLogin(User.Identity.Name);
            if (user == null)
            {
                return new UnauthorizedResult();
            }

            try
            {
                await foldersDataService.DeleteFolderById(id);
            }
            catch (Exception)
            {
                return new ConflictObjectResult("Что-то пошло не так...");
            }

            return Ok("Папка удалена.");
        }

    }
}