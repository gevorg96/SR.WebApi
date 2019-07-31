﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartRetail.App.DAL.BLL.DataServices;
using SmartRetail.App.DAL.BLL.Utils;
using SmartRetail.App.DAL.DropBox;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Helpers;
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
        private readonly IOrdersRepository ordersRepo;
        private readonly IStrategy strategy;
        private readonly ShopsChecker checker;
        private const string dropboxBasePath = "/dropbox/dotnetapi/products";

        #endregion

        #region Constructor
        public ProductService(IShopRepository _shopRepo, IBusinessRepository _businessRepo, IImageRepository _imgRepo, 
            IPictureWareHouse _dbBase, IProductRepository _prodRepo, IUnitRepository _unitRepo, IPriceRepository _priceRepo,
            ShopsChecker _checker, ICostRepository _costRepo, IStockRepository _stockRepo, IOrdersRepository ordersRepository, IStrategy _strategy)
        {
            shopRepo = _shopRepo;
            businessRepo = _businessRepo;
            imgRepo = _imgRepo;
            prodRepo = _prodRepo;
            unitRepo = _unitRepo;
            priceRepo = _priceRepo;
            costRepo = _costRepo;
            stockRepo = _stockRepo;
            ordersRepo = ordersRepository;
            strategy = _strategy;
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
                        var product = await prodRepo.GetByIdAsync(productViewModel.Id);
                        var img = await imgRepo.GetByIdAsync(product.id);
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
                    var product = await prodRepo.GetByIdAsync(productViewModel.Id);
                    var img = await imgRepo.GetByIdAsync(product.id);
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
        public async Task<IEnumerable<ProductViewModel>> GetProducts(UserProfile user)
        {
            var list = new List<ProductViewModel>();
            IEnumerable<Shop> shops;

            var prods = await prodRepo.GetProductsByBusinessAsync(user.business_id.Value);

            //create ViemModels
            foreach (var product in prods)
            {
                var cost = costRepo.GetByProdId(product.id).FirstOrDefault();
                var price = priceRepo.GetPriceByProdId(product.id);
                shops = user.shop_id == null ? shopRepo.GetShopsByBusiness(user.business_id.Value) : new List<Shop> { shopRepo.GetById(user.shop_id.Value) };
                var stocks = shops.Select(p => stockRepo.GetStockByShopAndProdIds(p.id, product.id));

                list.Add(new ProductViewModel
                {
                    Id = product.id,
                    ProdName = product.name,
                    Stock = stocks.Where(p => p != null).Sum(p => p.count).HasValue ? stocks.Where(p => p != null).Sum(p => p.count).Value : 0,
                    Cost = cost != null && cost.value.HasValue ? cost.value.Value : 0,
                    Price = price != null && price.price.HasValue ? price.price.Value : 0,
                    VendorCode = product.attr1,
                    ImgUrl = product.Image?.img_url_temp,
                    Color = product.attr10,
                    Size = product.attr9,
                    UnitId = product.unit_id.HasValue ? product.unit_id.Value : 0
                }) ;
            }

            return list;
        }

        public async Task<ProductViewModel> GetProduct(UserProfile user, int id)
        {
            var product = await prodRepo.GetByIdAsync(id);
            if (product == null)
            {
                return new ProductViewModel();
            }
            var img = await imgRepo.GetByIdAsync(id);
            var price = priceRepo.GetPriceByProdId(id);
            var cost = costRepo.GetByProdId(id).FirstOrDefault();
            var shops = user.shop_id == null ? shopRepo.GetShopsByBusiness(user.business_id.Value) : new List<Shop> { shopRepo.GetById(user.shop_id.Value) };
            var stocks = shops.Select(p => stockRepo.GetStockByShopAndProdIds(p.id, id));

            var prodVm = new ProductViewModel
            {
                Id = product.id,
                VendorCode = product.attr1,
                ProdName = product.name,
                Size = product.attr9,
                Color = product.attr10,
                UnitId = product.unit_id.HasValue ? product.unit_id.Value : 0,
                ImgUrl = img != null && !string.IsNullOrEmpty(img.img_url_temp) ? img.img_url_temp : "",
                Cost = cost != null && cost.value.HasValue ? cost.value.Value : 0,
                Price =  price != null && price.price.HasValue ? price.price.Value : 0,
                Stock = stocks.Sum(p => p.count).HasValue ? stocks.Sum(p => p.count).Value : 0
            };

            return prodVm;
        }

        /// <summary>
        /// Inserting product
        /// </summary>
        /// <param name="user">user info</param>
        /// <param name="product">product view model</param>
        public async Task<ProductDetailViewModel> AddProduct(UserProfile user, ProductDetailViewModel product)
        {
            //var shop = checker.GetCorrectShop(user, product);
            //if (shop == null) return;
            var business = await businessRepo.GetByIdAsync(user.business_id.Value);
            //create product model
            var prod = new Product
            {
                name = product.ProdName,
                business_id = business.id,
                unit_id = product.UnitId,
                attr1 = product.VendorCode,
                attr9 = product.Size,
                attr10 = product.Color
            };

            //add price link
            prod.Prices.Add(new Price { price = product.Price });

            //add with repo
            var pId = prodRepo.AddProduct(prod);

            if (product.Cost.HasValue)
            {
                foreach (var pair in product.Stocks)
                {
                    prod.Stock.Add(new Stock { count = pair.Stock, shop_id = pair.ShopId });
                    var dt = DateTime.Now;
                    var order = new Orders
                    {
                        report_date = dt,
                        shop_id = pair.ShopId,
                        isOrder = true,
                        OrderDetails = new List<OrderDetails>
                    {
                        new OrderDetails
                        {
                            prod_id = pId,
                            cost = product.Cost.Value,
                            count = pair.Stock
                        }
                    }
                    };
                    var id = await ordersRepo.AddOrderAsync(order);
                    var orderDal = (await ordersRepo.GetOrdersByShopIdInDateRange(order.shop_id, dt.AddSeconds(-1), dt)).FirstOrDefault(p => p.id == id);
                    await strategy.UpdateAverageCost(Direction.Order, orderDal);
                }
            }

            if (!string.IsNullOrEmpty(product.ImgBase64))
            {
                try
                {
                    using (var stream = product.img.OpenReadStream())
                    {
                        var bytes = ReadToEnd(stream);
                        var memory = new MemoryStream(bytes);
                        var imgParts = product.ImgBase64.Split(".");
                        var imgUrl = await dbBase.Upload(memory, "/products/" + business.id + ". " + business.name + "/" +
                            pId + "." + product.ProdName + "." + imgParts[imgParts.Length - 1]);
                        var img = new Images
                        {
                            img_url = imgUrl,
                            prod_id = pId,
                            img_name = product.ProdName,
                            img_type = imgParts[imgParts.Length - 1],
                            img_url_temp = ImageDataService.MakeTemporary(imgUrl),
                            img_path = product.Category
                        };
                        imgRepo.Add(img);
                    }
                }
                catch (Exception ex)
                {
                    product.Id = pId;
                    return product;
                }
            }

            product.Id = pId;
            return product;
        }

        private static byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }



        public async Task<ProductViewModel> UpdateProduct(UserProfile user, ProductDetailViewModel product)
        {
            var business = await businessRepo.GetByIdAsync(user.business_id.Value);

            int prodId;
            try
            {
                var pr = await prodRepo.GetByIdAsync(product.Id.Value, user.business_id.Value);
                if (pr != null)
                {
                    prodId = pr.id;
                }
                else
                {
                    throw new Exception("Не указан идентификатор товара / неверный идентификатор.");
                }
            }
            catch (Exception)
            {
                throw new Exception("Не указан идентификатор товара.");
            }


            var price = new Price
            {
                prod_id = prodId,
            };

            var prod = new Product
            {
                id = prodId,
                name = product.ProdName,
                business_id = user.business_id,
                unit_id = product.UnitId,
                attr1 = product.VendorCode,
                attr9 = product.Size,
                attr10 = product.Color
            };
            try
            {
                prodRepo.UpdateProduct(prod);
            }
            catch (Exception ex)
            {
                throw new Exception("Товар не был изменён.");
            }

            var candidatePrice = priceRepo.GetPriceByProdId(prodId);
            if (candidatePrice != null)
            {
                if (candidatePrice.price != product.Price)
                {
                    price.price = product.Price;

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

            if (product.img != null)
            {
                try
                {
                    var imgDal = await imgRepo.GetByIdAsync(prodId);
                    if (imgDal != null && !string.IsNullOrEmpty(imgDal.img_url))
                    {
                        var isDeleted = await dbBase.Delete("/products/" + business.id + ". " + business.name + "/" +
                            imgDal.prod_id + "." + imgDal.img_name + "." + imgDal.img_type);
                        if (!isDeleted)
                        {
                            throw new Exception("Невозможно заменить картинку.");
                        }
                    }

                    using (var stream = product.img.OpenReadStream())
                    {
                        var bytes = ReadToEnd(stream);
                        var memory = new MemoryStream(bytes);
                        var imgParts = product.ImgBase64.Split(".");
                        var imgUrl = await dbBase.Upload(memory, "/products/" + business.id+ ". " + business.name + "/" +
                            prodId + "." + product.ProdName + "." + imgParts[imgParts.Length - 1]);

                        imgDal.img_type = imgParts[imgParts.Length - 1];
                        imgDal.img_url = imgUrl;
                        imgDal.img_url_temp = ImageDataService.MakeTemporary(imgUrl);
                        imgDal.img_name = product.ProdName;
                        if (!string.IsNullOrEmpty(product.Category))
                            imgDal.img_path = product.Category;
                        await imgRepo.UpdateImage(imgDal);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Невозможно изменить картинку товара.");
                }
            }

            return await GetProduct(user, prodId);
        }

        public async Task<ProductDetailRequestViewModel> GetChoiceForUserAsync(UserProfile user)
        {
            var shopService = new ShopSerivce(shopRepo);
            var prdVm = new ProductDetailRequestViewModel();
            prdVm.Shops = shopService.GetStocks(user);
            var units = await unitRepo.GetAllUnitsAsync();
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
