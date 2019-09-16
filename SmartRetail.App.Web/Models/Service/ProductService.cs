using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SmartRetail.App.DAL.BLL.DataServices;
using SmartRetail.App.DAL.BLL.Utils;
using SmartRetail.App.DAL.DropBox;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Helpers;
using SmartRetail.App.DAL.Repository.Interfaces;
using SmartRetail.App.Web.Models.Interface;
using SmartRetail.App.Web.Models.Validation;
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

        private readonly IShopRepository _shopRepo;
        private readonly IBusinessRepository _businessRepo;
        private readonly IImageRepository _imgRepo;
        private readonly IPictureWareHouse _dbBase;
        private readonly IProductRepository _prodRepo;
        private readonly IUnitRepository _unitRepo;
        private readonly IPriceRepository _priceRepo;
        private readonly ICostRepository _costRepo;
        private readonly IStockRepository _stockRepo;
        private readonly IOrdersRepository _ordersRepo;
        private readonly IStrategy _strategy;
        private readonly IFoldersDataService _foldersDataService;
        private readonly ProductDataService _productDataService;

        #endregion

        #region Constructor
        public ProductService(IShopRepository _shopRepo, IBusinessRepository _businessRepo, IImageRepository _imgRepo, 
            IPictureWareHouse _dbBase, IProductRepository _prodRepo, IUnitRepository _unitRepo, IPriceRepository _priceRepo,
            ICostRepository _costRepo, IStockRepository _stockRepo, IOrdersRepository ordersRepository, 
            IStrategy _strategy,  IFoldersDataService _foldersDataService)
        {
            this._shopRepo = _shopRepo;
            this._businessRepo = _businessRepo;
            this._imgRepo = _imgRepo;
            this._prodRepo = _prodRepo;
            this._unitRepo = _unitRepo;
            this._priceRepo = _priceRepo;
            this._costRepo = _costRepo;
            this._stockRepo = _stockRepo;
            _ordersRepo = ordersRepository;
            this._strategy = _strategy;
            this._dbBase = _dbBase;
            this._foldersDataService = _foldersDataService;
            this._dbBase.GeneratedAuthenticationURL();
            this._dbBase.GenerateAccessToken();
            _productDataService = new ProductDataService(this._dbBase);
        }
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

            var prods = await _prodRepo.GetProductsByBusinessAsync(user.business_id.Value);

            //create ViemModels
            foreach (var product in prods)
            {
                var cost = _costRepo.GetByProdId(product.id).FirstOrDefault();
                var price = _priceRepo.GetPriceByProdId(product.id);
                shops = user.shop_id == null ? _shopRepo.GetShopsByBusiness(user.business_id.Value) : new List<Shop> { _shopRepo.GetById(user.shop_id.Value) };
                var stocks = shops.Select(p => _stockRepo.GetStockByShopAndProdIds(p.id, product.id));

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
            var product = await _prodRepo.GetByIdAsync(id);
            if (product == null)
            {
                return new ProductViewModel();
            }
            var img = await _imgRepo.GetByIdAsync(id);
            var price = _priceRepo.GetPriceByProdId(id);
            var cost = _costRepo.GetByProdId(id).FirstOrDefault();
            var shops = user.shop_id == null ? _shopRepo.GetShopsByBusiness(user.business_id.Value) : new List<Shop> { _shopRepo.GetById(user.shop_id.Value) };
            var stocks = shops.Select(p => _stockRepo.GetStockByShopAndProdIds(p.id, id)).Where(p => p != null).ToList();

            var prodVm = new ProductViewModel
            {
                Id = product.id,
                VendorCode = product.attr1,
                ProdName = product.name,
                Size = product.attr9,
                Color = product.attr10,
                UnitId = product.unit_id,
                ImgUrl = img != null && !string.IsNullOrEmpty(img.img_url_temp) ? img.img_url_temp : "",
                Cost = cost != null && cost.value.HasValue ? cost.value.Value : 0,
                Price =  price != null && price.price.HasValue ? price.price.Value : 0,
                Stock = stocks.Any() && stocks.Sum(p => p.count).HasValue ? stocks.Sum(p => p.count).Value : 0,
                CategoryId = product.folder_id
            };

            return prodVm;
        }

        public async Task<ProductDetailViewModel> AddProductTransaction(UserProfile user,
            ProductDetailViewModel product)
        {
            if (string.IsNullOrEmpty(product.ProdName))
            {
                throw new Exception("Наименование товара не может быть пустым.");
            }

            var business = await _businessRepo.GetByIdAsync(user.business_id.Value);

            var prod = new Product
            {
                name = product.ProdName,
                business_id = business.id,
                unit_id = product.UnitId,
                attr1 = product.VendorCode,
                attr9 = product.Size,
                attr10 = product.Color
            };
            prod.Price = new Price {price = product.Price};

            //await foldersDataService.AddFoldersByPath(product.Category, business.id);
            prod.folder_id = await _foldersDataService.GetFolderIdByPath(product.Category, business.id);

            if (!string.IsNullOrEmpty(product.ImgBase64))
            {
                using (var stream = product.img.OpenReadStream())
                {
                    var bytes = ReadToEnd(stream);
                    prod.ImgMemoryStream = new MemoryStream(bytes);
                    prod.ImgBase64 = product.ImgBase64;
                    prod.Category = product.Category;
                }
            }

            var dt = DateTime.Now;
            if (product.Cost.HasValue && product.Cost < 0)
            {
                throw new Exception("Себестоимость должна быть > 0.");
            }

            if (product.Cost.HasValue && product.Cost >= 0)
            {
                if (product.Stocks != null && product.Stocks.Any())
                {
                    foreach (var s in product.Stocks)
                    {
                        var order = new Order
                        {
                            isOrder = true,
                            report_date = dt,
                            shop_id = s.ShopId,
                            OrderDetails = new List<OrderDetail>
                            {
                                new OrderDetail
                                {
                                    //prod_id = pId,
                                    cost = product.Cost.Value,
                                    count = s.Stock
                                }
                            }
                        };

                        prod.Orders.Add(order);
                    }
                }
                else
                {
                    var cost = new Cost
                    {
                        value = product.Cost
                    };
                    prod.Cost.Add(cost);
                }
            }

            product.Id = await _productDataService.Insert(prod);
            return product;
        }

        public async Task<ProductViewModel> UpdateProductTransaction(UserProfile user, ProductDetailViewModel product)
        {
            var business = await _businessRepo.GetByIdAsync(user.business_id.Value);

            int prodId;
            try
            {
                var pr = await _prodRepo.GetByIdAsync(product.Id.Value, user.business_id.Value);
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
                throw new Exception("Не указан идентификатор товара / неверный идентификатор.");
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

            if (!string.IsNullOrEmpty(product.Category))
            {
                prod.folder_id = await _foldersDataService.GetFolderIdByPath(product.Category, business.id);
            }

            var candidatePrice = _priceRepo.GetPriceByProdId(prodId);
            candidatePrice.price = product.Price;
            prod.Price = candidatePrice;

            if (product.img != null)
            {
                try
                {
                    var imgDal = await _imgRepo.GetByIdAsync(prodId);
                    if (imgDal != null && !string.IsNullOrEmpty(imgDal.img_url))
                    {
                        var isDeleted = await _dbBase.Delete("/products/"+business.id + ". " + business.name + "/" +
                            imgDal.prod_id + "." + imgDal.img_name + "." + imgDal.img_type);
                        if (isDeleted)
                        {
                            imgDal.img_url = null;
                            imgDal.img_url_temp = null;
                            await _imgRepo.UpdateImage(imgDal);
                        }
                    }

                    using (var stream = product.img.OpenReadStream())
                    {
                        var bytes = ReadToEnd(stream);
                        prod.ImgMemoryStream = new MemoryStream(bytes);
                        prod.ImgBase64 = product.ImgBase64;
                        prod.Category = product.Category;
                        prod.Image = imgDal;
                    }
                }
                catch (Exception)
                {
                    throw new Exception("Невозможно удалить предыдущую картинку товара.");
                }
            }

            await _productDataService.Update(prod);

            return await GetProduct(user, prodId);
        }

        /// <summary>
        /// Inserting product
        /// </summary>
        /// <param name="user">user info</param>
        /// <param name="product">product view model</param>
        public async Task<ProductDetailViewModel> AddProduct(UserProfile user, ProductDetailViewModel product)
        {
            if (string.IsNullOrEmpty(product.ProdName))
            {
                throw new Exception("Наименование товара не может быть пустым.");
            }
            var business = await _businessRepo.GetByIdAsync(user.business_id.Value);
         
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
            int pId = 0;

            try
            {
                //await foldersDataService.AddFoldersByPath(product.Category, business.id);
                prod.folder_id = await _foldersDataService.GetFolderIdByPath(product.Category, business.id);
                //add with repo
                pId = _prodRepo.AddProduct(prod);
            }
            catch (Exception)
            {
                throw new Exception("Не получилось добавить товар в базу. Проверьте правильность заполнения полей.");
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
                        var imgUrl = await _dbBase.Upload(memory,
                            "/" + ". " + business.name + "/" +
                            pId + "." + product.ProdName + "." + imgParts[imgParts.Length - 1]);
                        var img = new Image
                        {
                            img_url = imgUrl,
                            prod_id = pId,
                            img_name = product.ProdName,
                            img_type = imgParts[imgParts.Length - 1],
                            img_url_temp = MakeTemporary(imgUrl),
                            img_path = product.Category
                        };
                        _imgRepo.Add(img);
                    }
                }
                catch (Exception)
                {
                    throw new Exception("Не получилось добавить картинку товара. Попробуйте снова.");
                }
            }

            var dt = DateTime.Now;
            if (product.Cost.HasValue && product.Cost < 0)
            {
                throw new Exception("Себестоимость должна быть > 0.");
            }
            if (product.Cost.HasValue && product.Cost >= 0)
            {
                if (product.Stocks != null && product.Stocks.Any())
                {
                    foreach (var s in product.Stocks)
                    {
                        var order = new Order
                        {
                            isOrder = true,
                            report_date = dt,
                            shop_id = s.ShopId,
                            OrderDetails = new List<OrderDetail>
                            {
                                new OrderDetail
                                {
                                    prod_id = pId,
                                    cost = product.Cost.Value,
                                    count = s.Stock
                                }
                            }
                        };

                        try
                        {
                            var orderId = await _ordersRepo.AddOrderAsync(order);
                            var orderDal = await _ordersRepo.GetByIdWithMultiAsync(orderId);
                            await _strategy.UpdateAverageCost(Direction.Order, orderDal);
                        }
                        catch (Exception)
                        {
                            throw new Exception("Не удалось добавить остаток на склад.");
                        }
                    }
                }
                else
                {
                    var cost = new Cost
                    {
                        prod_id = pId,
                        value = product.Cost
                    };
                    await _costRepo.AddCostAsync(cost);
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
            var business = await _businessRepo.GetByIdAsync(user.business_id.Value);

            int prodId;
            try
            {
                var pr = await _prodRepo.GetByIdAsync(product.Id.Value, user.business_id.Value);
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
                throw new Exception("Не указан идентификатор товара / неверный идентификатор.");
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

            if (!string.IsNullOrEmpty(product.Category))
            {
                prod.folder_id = await _foldersDataService.GetFolderIdByPath(product.Category, business.id);
            }

            try
            {
                _prodRepo.UpdateProduct(prod);
            }
            catch (Exception)
            {
                throw new Exception("Товар не был изменён.");
            }

            var candidatePrice = _priceRepo.GetPriceByProdId(prodId);
            if (candidatePrice != null)
            {
                if (candidatePrice.price != product.Price)
                {
                    price.price = product.Price;

                    try
                    {
                        _priceRepo.Update(price);
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
                    _priceRepo.Add(price);
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
                    var imgDal = await _imgRepo.GetByIdAsync(prodId);
                    if (imgDal != null && !string.IsNullOrEmpty(imgDal.img_url))
                    {
                        var isDeleted = await _dbBase.Delete("/" + business.name + "/" +
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
                        var imgUrl = await _dbBase.Upload(memory, "/" + business.name + "/" +
                            prodId + "." + product.ProdName + "." + imgParts[imgParts.Length - 1]);

                        imgDal.img_type = imgParts[imgParts.Length - 1];
                        imgDal.img_url = imgUrl;
                        imgDal.img_url_temp = MakeTemporary(imgUrl);
                        imgDal.img_name = product.ProdName;
                        if (!string.IsNullOrEmpty(product.Category))
                        {
                            imgDal.img_path = product.Category;
                        }

                        await _imgRepo.UpdateImage(imgDal);
                    }
                }
                catch (Exception)
                {
                    throw new Exception("Невозможно изменить картинку товара.");
                }
            }

            return await GetProduct(user, prodId);
        }

        public async Task<ProductDetailRequestViewModel> GetChoiceForUserAsync(UserProfile user)
        {
            var shopService = new ShopSerivce(_shopRepo);
            var prdVm = new ProductDetailRequestViewModel();
            prdVm.Shops = shopService.GetStocks(user);
            var units = await _unitRepo.GetAllUnitsAsync();
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
            var business = _businessRepo.GetById(user.business_id.Value);
            Shop shop = null;

            //get shop
            if (user.shop_id != null && user.shop_id.Value != 0)
                shop = _shopRepo.GetById(user.shop_id.Value);

            //make path in dropbox
            var path =  "/products/" + business.id + ". " + business.name;

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
