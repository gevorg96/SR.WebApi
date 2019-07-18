using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SmartRetail.App.DAL.BLL.DataServices;
using SmartRetail.App.DAL.DropBox;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Repository;
using SmartRetail.App.DAL.Repository.Interfaces;
using SmartRetail.App.Web.Models.Interface;
using SmartRetail.App.Web.Models.Validation;
using SmartRetail.App.Web.Models.ViewModel;
using SmartRetail.App.Web.Models.ViewModel.Products;
using SmartRetail.App.Web.Models.ViewModel.Units;

namespace SmartRetail.App.Web.Models.Service
{
    /// <summary>
    /// Service for ViewModels, which by user create 
    /// instances and return them
    /// </summary>
    public class ProductService : IProductService
    {
        #region Private fields

        private readonly IShopRepository shopRepo;
        private readonly IBusinessRepository businessRepo;
        private readonly IImageRepository imgRepo;
        private readonly IPictureWareHouse dbBase;
        private readonly IProductRepository prodRepo;
        private readonly IUnitRepository unitRepo;
        private readonly IPriceRepository priceRepo;
        private readonly ICostRepository costRepo;
        private readonly IStockRepository stockRepo;
        private readonly ShopsChecker checker;
        private const string dropboxBasePath = "/dropbox/dotnetapi/products";

        #endregion

        #region Constructor
        public ProductService(IShopRepository _shopRepo, IBusinessRepository _businessRepo, IImageRepository _imgRepo, 
            IPictureWareHouse _dbBase, IProductRepository _prodRepo, IUnitRepository _unitRepo, IPriceRepository _priceRepo,
            ShopsChecker _checker, ICostRepository _costRepo, IStockRepository _stockRepo)
        {
            shopRepo = _shopRepo;
            businessRepo = _businessRepo;
            imgRepo = _imgRepo;
            prodRepo = _prodRepo;
            unitRepo = _unitRepo;
            priceRepo = _priceRepo;
            costRepo = _costRepo;
            stockRepo = _stockRepo;
            dbBase = _dbBase;
            checker = _checker;
            dbBase.GeneratedAuthenticationURL();
            dbBase.GenerateAccessToken();
        }
        #endregion

        #region Folders

        public void ChangeShop(UserProfile user, ChangeDestinationViewModel model)
        {
            var products = prodRepo.GetProductsByIds(model.productsCount.Select(p => p.prodId));
            //products = products.Where(p => p.shop_id == shopId).ToList();

        }

        public void ChangePath(UserProfile user, int prodId, string newPath)
        {
            
        }

        #region Get Next Level Folders & Files

        /// <summary>
        /// Get next level of product's hierarchy
        /// </summary>
        /// <param name="user">user info</param>
        /// <param name="fullpath">path of folder which subfolders we need to return</param>
        /// <param name="needProducts">flag that on/off ProductViewModel in response</param>
        /// <returns></returns>
        public async Task<ProductGroupViewModel> GetNexLevelGroup(UserProfile user, string fullpath = null, bool needProducts = true)
        {
            var prodGroup = new ProductGroupViewModel();

            var path = GetPath(fullpath, user);
            
            var items = await dbBase.GetAllFolders(path, false);
            prodGroup.Folders = items.Entries.Where(p => p.IsFolder)
                .Select(p => new FolderViewModel {folder = p.Name, fullpath = p.PathLower}).ToList();

            if (needProducts)
            {
                var pics = items.Entries.Where(p => p.IsFile)
                    .Select(p => new ProductViewModel { Id = Convert.ToInt32(p.Name.Split('.')[0]) }).OrderBy(p => p.Id).ToList();

                if (pics.Any())
                {
                    var rnd = new Random();
                    prodGroup.Products = new List<ProductViewModel>();
                    foreach (var productViewModel in pics)
                    {
                        var product = prodRepo.GetById(productViewModel.Id);
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
                            Size = product.attr9
                        });
                    }
                }
            }
            
            return prodGroup;
        }

        #endregion

        #region Search Files and Folders
  
        /// <summary>
        /// Search folders and products
        /// </summary>
        /// <param name="user">user info</param>
        /// <param name="name">pattern for searching</param>
        /// <param name="start"></param>
        /// <param name="limit"></param>
        /// <param name="path">path for searching in</param>
        /// <returns></returns>
        public async Task<ProductGroupViewModel> Search(UserProfile user, string name, ulong start, ulong limit, string path = null)
        {
            var prodGroup = new ProductGroupViewModel();

            //validate path
            path = GetPath(path, user);

            //dropbox.search
            var items = await dbBase.SearchFolder(path, name, start, limit);

            //get folders from result
            prodGroup.Folders = items.Matches.Where(p => p.Metadata.IsFolder)
                .Select(p => new FolderViewModel { folder = p.Metadata.Name, fullpath = p.Metadata.PathLower }).ToList();

            //get products from result
            var pics = items.Matches.Where(p => p.Metadata.IsFile)
                .Select(p => new ProductViewModel { Id = Convert.ToInt32(p.Metadata.Name.Split('.')[0]) }).OrderBy(p => p.Id).ToList();

            if (pics.Any())
            {
                var rnd = new Random();
                prodGroup.Products = new List<ProductViewModel>();
                foreach (var productViewModel in pics)
                {
                    var product = prodRepo.GetById(productViewModel.Id);
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
                        Size = product.attr9
                    });
                }
            }

            return prodGroup;
        }

        #endregion

        #endregion

        #region Products

        /// <summary>
        /// Method that gets products
        /// </summary>
        /// <param name="user">user info</param>
        /// <returns></returns>
        public IEnumerable<ProductViewModel> GetProducts(UserProfile user)
        {
            var list = new List<ProductViewModel>();
            List<Shop> shops = new List<Shop>();

            if (user.shop_id.HasValue && user.shop_id != 0)
            {
                //get shop with products
                shops = new List<Shop> { shopRepo.GetShopMultiMappingProducts(user.shop_id.Value) };
            }
            else
            {
                var business = businessRepo.GetWithFilter("id", user.business_id.ToString()).FirstOrDefault();

                if (business != null)
                {
                    //get shops with products
                    shops = shopRepo.GetShopsByBusinessMultiMappingProducts(business.id).ToList();
                }
            }

            var products = new List<Product>();
            if (shops != null && shops.Any())
            {
                foreach (var shop in shops)
                {
                    //add products in list from shop(s)
                    products.AddRange(shop.Product);
                }
            }

            //todo
            var rnd = new Random();

            //create ViemModels
            foreach (var product in products)
            {
                list.Add(new ProductViewModel
                {
                    Id = product.id,
                    ProdName = product.name,
                    Stock = rnd.Next(1, 30),
                    Cost = rnd.Next(1000, 10000),
                    Price = rnd.Next(2000, 20000),
                    VendorCode = product.attr1,
                    ImgUrl = product.Image?.img_url_temp,
                    Color = product.attr10,
                    Size = product.attr9
                });
            }

            return list;

        }

        public ProductViewModel GetProduct(UserProfile user, int id)
        {
            var product = prodRepo.GetById(id);
            if (product == null)
            {
                return new ProductViewModel();
            }
            var img = imgRepo.GetById(id);
            var prodVm = new ProductViewModel
            {
                Id = product.id,
                VendorCode = product.attr1,
                ProdName = product.name,
                Size = product.attr9,
                Color = product.attr10,
                UnitId = product.unit_id.HasValue ? product.unit_id.Value : 0,
                ImgUrl = img != null && !string.IsNullOrEmpty(img.img_url_temp) ? img.img_url_temp : ""
            };

            if (product.business_id == user.business_id)
            {
                if (user.shop_id.HasValue && user.shop_id == product.shop_id)
                {
                    var cost = costRepo.GetByProdAndShopIds(id, user.shop_id.Value);
                    var price = priceRepo.GetPriceByProdAndShopIds(id, user.shop_id.Value);
                    var stock = stockRepo.GetStockByShopAndProdIds(user.shop_id.Value, id);
                    prodVm.Cost = cost != null && cost.value.HasValue ? cost.value.Value : 0;
                    prodVm.Price = price != null && price.price.HasValue ? price.price.Value : 0;
                    prodVm.Stock = stock != null && stock.count.HasValue ? stock.count.Value : 0;
                    return prodVm;
                }
                if (user.shop_id.HasValue && user.shop_id != product.shop_id)
                {
                    throw new Exception("Вы не можете просматривать этот товар.");
                }
                if (!user.shop_id.HasValue)
                {
                    throw new Exception("Необходимо выбрать магазин.");
                }
            }
            throw new Exception("Вы не можете просматривать этот товар.");
        }

        /// <summary>
        /// Inserting product
        /// </summary>
        /// <param name="user">user info</param>
        /// <param name="product">product view model</param>
        public async Task AddProduct(UserProfile user, ProductDetailViewModel product)
        {
            var shop = checker.GetCorrectShop(user, product);
            if (shop == null) return;
            
            //create product model
            var prod = new Product
            {
                name = product.ProdName,
                shop_id = shop?.id ?? product.ShopId,
                business_id = shop!= null ? shop.business_id: shop.Business.id,
                unit_id = product.UnitId,
                attr1 = product.VendorCode,
                attr9 = product.Size,
                attr10 = product.Color
            };

            //add price link
            prod.Prices.Add(new Price { price = product.Price });

            //add cost link
            prod.Cost.Add(new Cost { value = product.Cost });

            //add stock link
            prod.Stock.Add(new Stock { count = product.Stock });

            //add with repo
            var pId = prodRepo.AddProduct(prod);
            var bytes = Convert.FromBase64String(product.ImgBase64);
            var contents = new MemoryStream(bytes);
            var path = product.Category + "/" + pId + ". " + product.ProdName + ".jpg";
            var imgUrl = await dbBase.Upload(contents, path);
            var img = new Images
            {
                img_url = imgUrl,
                prod_id = pId,
                img_name = product.ProdName,
                img_type = "jpg",
                img_url_temp = ImageDataService.MakeTemporary(imgUrl),
                img_path = product.Category
            };
            imgRepo.Add(img);
        }

        public async Task UpdateProduct(UserProfile user, ProductDetailViewModel product)
        {
            var shop = checker.GetCorrectShop(user, product);
            if (shop == null) return;

            int prodId;
            try
            {
                prodId = product.Id.Value;
            }
            catch (Exception)
            {
                throw new Exception("Не указан идентификатор товара.");
            }


            var price = new Price
            {
                prod_id = prodId,
                shop_id = product.ShopId
            };

            if (!string.IsNullOrEmpty(product.Category))
            {
                var photo = imgRepo.GetById(prodId);
                var path = await dbBase.GetFileWithSharedLink(photo.img_url);
                var parts = path.Split("/");
                var name = parts[parts.Length - 1];
                try
                {
                    var res = await dbBase.MoveFile(path, product.Category + "/" + name);
                }
                catch (Exception)
                {
                    throw new Exception("Невозможно изменить категорию.");
                }
            }

            var prod = new Product
            {
                id = prodId,
                name = product.ProdName,
                shop_id = shop?.id ?? product.ShopId,
                business_id = shop != null ? shop.business_id : shop.Business.id,
                unit_id = product.UnitId,
                attr1 = product.VendorCode,
                attr9 = product.Size,
                attr10 = product.Color
            };
            try
            {
                prodRepo.UpdateProduct(prod);
            }
            catch (Exception)
            {
                throw new Exception("Товар не был изменён.");
            }

            var candidatePrice = priceRepo.GetPriceByProdId(prodId);
            if (candidatePrice != null)
            {
                if (candidatePrice.price != product.Price)
                {
                    price.price = product.Price;

                    if (candidatePrice.shop_id != product.ShopId)
                    {
                        price.shop_id = product.ShopId;
                        try
                        {
                            priceRepo.Update(price);
                        }
                        catch (Exception)
                        {
                            throw new Exception("Не получилось изменить цену товара.");
                        }
                    }
                }
            }
            else
            {
                price.price = product.Price;
                try
                {
                    priceRepo.Add(price);
                }
                catch (Exception)
                {
                    throw new Exception("Не получилось добавить цену товара.");
                }
            }
        }

        public ProductDetailRequestViewModel GetChoiceForUser(UserProfile user)
        {
            var shopService = new ShopSerivce(shopRepo);
            var prdVm = new ProductDetailRequestViewModel();
            prdVm.Shops = shopService.GetStocks(user);
            var units = unitRepo.GetAllUnits();
            prdVm.Units = units.Select(p => new UnitViewModel {id = p.id, name = p.value}).ToList();
            return prdVm;
        }
        
        #endregion

        #region Additional Methods

        private static string MakeTemporary(string link)
        {
            return link.Replace("https://www", "https://dl").Replace("?dl=0", "?dl=1");

        }

        /// <summary>
        /// Method that validate incoming path
        /// </summary>
        /// <param name="fullpath">path from client</param>
        /// <param name="user">user info</param>
        /// <returns></returns>
        private string GetPath(string fullpath, UserProfile user)
        {
            //get business
            var business = businessRepo.GetById(user.business_id.Value);
            Shop shop = null;

            //get shop
            if (user.shop_id != null && user.shop_id.Value != 0)
                shop = shopRepo.GetById(user.shop_id.Value);

            //make path in dropbox
            var path = dropboxBasePath + "/" + business.id + ". " + business.name;

            if (string.IsNullOrEmpty(fullpath))
            {
                if (shop != null)
                    path += "/" + shop.id + ". " + shop.name;
            }
            else
            {
                // validation
                if (shop == null && fullpath.StartsWith(path.ToLower()))
                {
                    path = fullpath;
                }
                else if (shop != null && fullpath.StartsWith(path.ToLower() + "/" + shop.id + ". " + shop.name.ToLower()))
                {
                    path = fullpath;
                }
                else
                {
                    if (shop != null)
                    {
                        path += "/" + shop.id + ". " + shop.name;
                    }
                }
            }

            return path;
        }

        #endregion
    }
}
