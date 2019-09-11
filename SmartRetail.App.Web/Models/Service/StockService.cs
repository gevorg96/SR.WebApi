using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Repository.Interfaces;
using SmartRetail.App.Web.Models.Interface;
using SmartRetail.App.Web.Models.Validation;
using SmartRetail.App.Web.Models.ViewModel.Products;

namespace SmartRetail.App.Web.Models.Service
{
    public class StockService: IStockService
    {
        private IStockRepository stockRepo;
        private readonly IPriceRepository priceRepo;
        private readonly ICostRepository costRepo;
        private readonly ShopsChecker _shopsChecker;

        public StockService(IStockRepository stockRepo, ShopsChecker shopsChecker,
            IPriceRepository _priceRepo, ICostRepository _costRepo)
        {
            this.stockRepo = stockRepo;
            _shopsChecker = shopsChecker;
            priceRepo = _priceRepo;
            costRepo = _costRepo;
        }
        
        public async Task<IEnumerable<ProductViewModel>> GetStocks(UserProfile user, int? shopId)
        {
            var list = new List<ProductViewModel>();
            IEnumerable<Stock> stocks = new List<Stock>();

            var avl = _shopsChecker.CheckAvailability(user, shopId);
            if(!avl.isCorrectShop)
                return new List<ProductViewModel>();
            if (!avl.hasShop && avl.isAdmin)
                stocks = stockRepo.GetStocksWithProductsByBusiness(user.business_id.Value).ToList();
            else if(!avl.hasShop && !avl.isAdmin)
                return new List<ProductViewModel>();
            else if(avl.hasShop)
                stocks = await stockRepo.GetStocksWithProducts(shopId.Value);
           
            if (stocks != null && stocks.Any())
            {
                foreach (var stock in stocks)
                {
                    var product = stock.Product;
                    var cost = costRepo.GetByProdId(product.id).FirstOrDefault();
                    var price = priceRepo.GetPriceByProdId(product.id);
                    if (stock.count > 0)
                    {
                        list.Add(new ProductViewModel
                        {
                            Id = product.id,
                            ProdName = product.name,
                            Stock = Convert.ToDecimal(stock.count),
                            Cost = cost != null && cost.value.HasValue ? cost.value.Value : 0,
                            Price = price != null && price.price.HasValue ? price.price.Value : 0,
                            VendorCode = product.attr1,
                            ImgUrl = product.Image?.img_url_temp,
                            Color = product.attr10,
                            Size = product.attr9,
                            UnitId = product.unit_id.HasValue ? product.unit_id.Value : 0
                        });
                    }
                }
            }

            return list;
        }
    }
}