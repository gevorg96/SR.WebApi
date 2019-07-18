using System;
using System.Collections.Generic;
using System.Linq;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Repository;
using SmartRetail.App.Web.Models.Interface;
using SmartRetail.App.Web.Models.Validation;

namespace SmartRetail.App.Web.Models.Service
{
    public class StockService: IStockService
    {
        private IShopRepository shopRepo;
        private IBusinessRepository businessRepo;
        private IStockRepository stockRepo;
        private readonly ShopsChecker _shopsChecker;

        public StockService(IShopRepository shopRepo, IBusinessRepository businessRepo, IStockRepository stockRepo, ShopsChecker shopsChecker)
        {
            this.shopRepo = shopRepo;
            this.businessRepo = businessRepo;
            this.stockRepo = stockRepo;
            _shopsChecker = shopsChecker;
        }
        
        public IEnumerable<ProductViewModel> GetStocks(UserProfile user, int? shopId)
        {
            var list = new List<ProductViewModel>();
            IEnumerable<Stock> stocks = new List<Stock>();

            var avl = _shopsChecker.CheckAvailability(user, shopId);
            if(!avl.isCorrectShop)
                return new List<ProductViewModel>();
            if (!avl.hasShop && avl.isAdmin)
            {   
                stocks = stockRepo.GetStocksWithProductsByBusiness(user.business_id.Value).ToList();
            }
            else if(!avl.hasShop && !avl.isAdmin)
            {
                return new List<ProductViewModel>();
            }
            else if(avl.hasShop)
            {
                stocks = stockRepo.GetStocksWithProducts(shopId.Value);
            }
            
            var rnd = new Random();
            
            if (stocks != null && stocks.Any())
            {
                foreach (var stock in stocks)
                {
                    var product = stock.Product;
                    if (stock.count > 0)
                    {
                        list.Add(new ProductViewModel
                        {
                            Id = product.id,
                            ProdName = product.name,
                            Stock = Convert.ToDecimal(stock.count),
                            Cost = rnd.Next(1000, 10000),
                            Price = rnd.Next(2000, 20000),
                            VendorCode = product.attr1,
                            ImgUrl = product.Image?.img_url_temp,
                            Color = product.attr10,
                            Size = product.attr9
                        });
                    }
                }
            }

            return list;
        }
    }
}