using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartRetail.App.DAL.BLL;
using SmartRetail.App.DAL.BLL.DataStructures;
using SmartRetail.App.DAL.BLL.HelperClasses;
using SmartRetail.App.DAL.BLL.StructureFillers;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Repository;
using SmartRetail.App.Web.Models.Interface;
using SmartRetail.App.Web.Models.ViewModel;

namespace SmartRetail.App.Web.Models.Service
{
    public class CategoryService : ICategoryService
    {
        private ITreeFiller filler;
        private readonly IBusinessRepository businessRepo;
        private readonly IProductRepository productRepo;
        private readonly IImageRepository imgRepo;

        public CategoryService(ITreeFiller _filler, IBusinessRepository brepo, IProductRepository _productRepo, IImageRepository _imgRepo)
        {
            filler = _filler;
            businessRepo = brepo;
            productRepo = _productRepo;
            imgRepo = _imgRepo;
        }

        public async Task<ProductGroupViewModel> GetNexLevelGroup(UserProfile user, string fullpath = null, bool needProducts = true)
        {
            var prodGroup = new ProductGroupViewModel();
            var businessTask = Task.Run(() => businessRepo.GetByIdAsync(user.business_id.Value));
            var fillerTask = Task.Run(() => filler.FillTreeByBusinessAsync(user.business_id.Value));
            var (tree, business) = await Tasker.WhenAll(fillerTask, businessTask);

            IEnumerable<ImgTwinModel> level = new List<ImgTwinModel>();

            if (string.IsNullOrEmpty(fullpath))
            {
                level = filler.GetLevel("/products/" + business.id + ". " + business.name);
            }
            else
            {
                level = filler.GetLevel(fullpath);
            }
            
            prodGroup.Folders = level.Where(p => p.isFolder)
                .Select(p => new FolderViewModel { folder = p.folder, fullpath = p.fullpath }).ToList();

            if (needProducts)
            {
                var pics = level.Where(p => p.isFile)
                    .Select(p => new ProductViewModel { Id = Convert.ToInt32(p.folder.Split('.')[0]) }).OrderBy(p => p.Id).ToList();

                if (pics.Any())
                {
                    var rnd = new Random();
                    prodGroup.Products = new List<ProductViewModel>();
                    foreach (var productViewModel in pics)
                    {
                        var product = await productRepo.GetByIdAsync(productViewModel.Id);
                        var img = imgRepo.GetById(product.id);
                        prodGroup.Products.Add(new ProductViewModel
                        {
                            Id = product.id,
                            ProdName = product.name,
                            Stock = rnd.Next(1, 30),
                            Cost = rnd.Next(1000, 10000),
                            Price = rnd.Next(2000, 20000),
                            VendorCode = product.attr1,
                            ImgUrl = img?.img_url_temp,
                            Color = product.attr10,
                            Size = product.attr9,
                            UnitId = product.unit_id.HasValue ? product.unit_id.Value : 0
                           
                        });
                    }
                }
            }

            return prodGroup;
        }

        public async Task<ProductGroupViewModel> Search(UserProfile user, string name, string path = null)
        {
            var businessTask = Task.Run(() => businessRepo.GetByIdAsync(user.business_id.Value));
            var fillerTask = Task.Run(() => filler.FillTreeByBusinessAsync(user.business_id.Value));
            var (tree, business) = await Tasker.WhenAll(fillerTask, businessTask);

            var bname = business.id + ". " + business.name;

            var prodGroup = new ProductGroupViewModel();
            CathegoryTree<ImgTwinModel> subTree = null;
            IEnumerable<ImgTwinModel> result = new List<ImgTwinModel>();

            if (!string.IsNullOrEmpty(path))
            {
                subTree = filler.SearchSubTree(path);
            }

            if (!string.IsNullOrEmpty(name))
            {
                result = filler.Search(name, subTree != null ? subTree : filler.Tree);
            }
            result = result.Where(p => p.fullpath.Split('/')[2] == bname).ToList();

            //get folders from result
            prodGroup.Folders = result.Where(p => p.isFolder)
                .Select(p => new FolderViewModel { folder = p.folder, fullpath = p.fullpath }).ToList();

            //get products from result
            var pics = result.Where(p => p.isFile)
                .Select(p => new ProductViewModel { Id = Convert.ToInt32(p.folder.Split('.')[0]) }).OrderBy(p => p.Id).ToList();

            if (pics.Any())
            {
                var rnd = new Random();
                prodGroup.Products = new List<ProductViewModel>();
                foreach (var productViewModel in pics)
                {
                    var product = await productRepo.GetByIdAsync(productViewModel.Id);
                    var img = imgRepo.GetById(product.id);
                    prodGroup.Products.Add(new ProductViewModel
                    {
                        Id = product.id,
                        ProdName = product.name,
                        Stock = rnd.Next(1, 30),
                        Cost = rnd.Next(1000, 10000),
                        Price = rnd.Next(2000, 20000),
                        VendorCode = product.attr1,
                        ImgUrl = img?.img_url_temp,
                        Color = product.attr10,
                        Size = product.attr9,
                        UnitId = product.unit_id.HasValue ? product.unit_id.Value : 0
                    });
                }
            }

            return prodGroup;
        }
    }
}
