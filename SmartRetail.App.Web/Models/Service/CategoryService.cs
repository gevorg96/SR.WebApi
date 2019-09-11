using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartRetail.App.DAL.BLL;
using SmartRetail.App.DAL.BLL.DataServices;
using SmartRetail.App.DAL.BLL.HelperClasses;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Helpers;
using SmartRetail.App.DAL.Repository.Interfaces;
using SmartRetail.App.Web.Models.Interface;
using SmartRetail.App.Web.Models.ViewModel;
using SmartRetail.App.Web.Models.ViewModel.Folders;
using SmartRetail.App.Web.Models.ViewModel.Products;

namespace SmartRetail.App.Web.Models.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly IBusinessRepository businessRepo;
        private readonly IImageRepository imgRepo;
        private readonly IProductService productService;
        private readonly IFoldersDataService foldersDs;
        public CategoryService( IBusinessRepository brepo, IImageRepository _imgRepo, IProductService _productService, IFoldersDataService _foldersDs)
        {
            businessRepo = brepo;
            imgRepo = _imgRepo;
            productService = _productService;
            foldersDs = _foldersDs;
        }

        public async Task<Tree<ImgTwinModel>> GetFullFolderTree(UserProfile user)
        {
            return await foldersDs.GetFoldersTreeAsync(user.business_id.Value);
        }

        public async Task<ProductGroupViewModel> GetNexLevelGroup(UserProfile user, string fullpath = null, bool needProducts = true)
        {
            var prodGroup = new ProductGroupViewModel();
            var businessTask = Task.Run(() => businessRepo.GetByIdAsync(user.business_id.Value));
            var fillerTask = Task.Run(() => foldersDs.GetTreeAsync(user.business_id.Value));
            var (tree, business) = await Tasker.WhenAll(fillerTask, businessTask);

            IEnumerable<ImgTwinModel> level = new List<ImgTwinModel>();

            if (string.IsNullOrEmpty(fullpath))
            {
                level = foldersDs.GetLevel("/" + business.name);
            }
            else
            {
                level = foldersDs.GetLevel(fullpath);
            }
            
            prodGroup.Folders = level.Where(p => p.isFolder)
                .Select(p => new FolderViewModel { id = p.id, folder = p.folder, fullpath = p.fullpath }).ToList();

            if (needProducts)
            {
                var pics = level.Where(p => p.isFile)
                    .Select(p => new ProductViewModel { Id = Convert.ToInt32(p.folder.Split('.')[0]) }).OrderBy(p => p.Id).ToList();

                if (pics.Any())
                {
                    prodGroup.Products = new List<ProductViewModel>();
                    foreach (var productViewModel in pics)
                    {
                        prodGroup.Products.Add(await productService.GetProduct(user,productViewModel.Id));
                    }
                }
            }

            return prodGroup;
        }

        public async Task<ProductGroupViewModel> Search(UserProfile user, string name, string path = null)
        {
            var businessTask = Task.Run(() => businessRepo.GetByIdAsync(user.business_id.Value));
            var fillerTask = Task.Run(() => foldersDs.GetTreeAsync(user.business_id.Value));
            var (tree, business) = await Tasker.WhenAll(fillerTask, businessTask);

            var bname = business.id + ". " + business.name;

            var prodGroup = new ProductGroupViewModel();
            Tree<ImgTwinModel> subTree = null;
            IEnumerable<ImgTwinModel> result = new List<ImgTwinModel>();

            if (!string.IsNullOrEmpty(path))
            {
                subTree = foldersDs.SearchSubTree(path);
            }

            if (!string.IsNullOrEmpty(name))
            {
                result = foldersDs.Search(subTree ?? foldersDs.Tree, name);
            }
            //result = result.Where(p => p.fullpath.Split('/')[2] == bname).ToList();

            //get folders from result
            prodGroup.Folders = result.Where(p => p.isFolder)
                .Select(p => new FolderViewModel { id = p.id, folder = p.folder, fullpath = p.fullpath }).ToList();

            //get products from result
            var pics = result.Where(p => p.isFile)
                .Select(p => new ProductViewModel { Id = p.id }).OrderBy(p => p.Id).ToList();

            if (pics.Any())
            {
                prodGroup.Products = new List<ProductViewModel>();
                foreach (var productViewModel in pics)
                {
                    prodGroup.Products.Add(await productService.GetProduct(user, productViewModel.Id));
                }
            }

            return prodGroup;
        }
    }
}
