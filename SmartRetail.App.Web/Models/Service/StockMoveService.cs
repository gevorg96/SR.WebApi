using SmartRetail.App.DAL.BLL.Utils;
using SmartRetail.App.DAL.DropBox;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Helpers;
using SmartRetail.App.DAL.Repository.Interfaces;
using SmartRetail.App.Web.Models.Interface;
using SmartRetail.App.Web.Models.Validation;
using SmartRetail.App.Web.Models.ViewModel.StockMove;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartRetail.App.Web.Models.Service
{
    public class StockMoveService : IStockMoveService
    {
        #region Private fields

        private readonly IPictureWareHouse dbBase;
        private readonly ICostRepository costRepo;
        private readonly IStockRepository stockRepo;
        private readonly IOrdersRepository ordersRepo;
        private readonly IStrategy strategy;

        #endregion

        #region Constructor
        public StockMoveService(IPictureWareHouse _dbBase, ICostRepository _costRepo, IStockRepository _stockRepo, IOrdersRepository ordersRepository, IStrategy _strategy)
        {
            costRepo = _costRepo;
            stockRepo = _stockRepo;
            ordersRepo = ordersRepository;
            strategy = _strategy;
            dbBase = _dbBase;
            dbBase.GeneratedAuthenticationURL();
            dbBase.GenerateAccessToken();
        }
        #endregion


        public async Task<IEnumerable<StockMoveViewModel>> GetProducts(UserProfile user, int shopId, string name)
        {
            var list = new List<StockMoveViewModel>();
            IEnumerable<Stock> stocks = new List<Stock>();

            if (user.shop_id == null || user.shop_id == shopId)
            {
                stocks = await stockRepo.GetStocksWithProducts(shopId);
            }
            else
            {
                return new List<StockMoveViewModel>();
            }

            if (!string.IsNullOrEmpty(name))
            {
                stocks = stocks.Where(p => p.Product != null && p.Product.name.ToLowerInvariant().StartsWith(name.ToLowerInvariant()));
            }

            if (stocks != null && stocks.Any())
            {
                foreach (var stock in stocks)
                {
                    var product = stock.Product;
                    if (stock.count > 0)
                    {
                        list.Add(new StockMoveViewModel
                        {
                            id = product.id,
                            name = product.name,
                            stock = Convert.ToDecimal(stock.count),
                            vendorCode = product.attr1,
                            imgUrl = product.Image?.img_url_temp,
                        });
                    }
                }
            }

            return list;
        }

        public async Task MoveStocks(UserProfile user, StockMoveRequestViewModel model)
        {
            var dtNow = DateTime.Now;
            var order = new Order
            {
                isOrder = false,
                report_date = dtNow,
                shop_id = model.shopFrom
            };

            order.OrderDetails = model.products.Select(p => new OrderDetail
            {
                prod_id = p.id,
                cost = 0,
                count = p.value
            }).ToList();

            try
            {
                var id = await ordersRepo.AddCancellationAsync(order);
                var orderDal = (await ordersRepo.GetCancellationsByShopIdInDateRange(order.shop_id, dtNow.AddSeconds(-1), dtNow)).Last(p => p.id == id);

                foreach (var orderDetail in order.OrderDetails)
                {
                    var cost = costRepo.GetByProdId(orderDetail.prod_id).FirstOrDefault();
                    orderDetail.cost = cost != null && cost.value.HasValue ? cost.value.Value : 0;
                }

                await strategy.UpdateAverageCost(Direction.Cancellation, orderDal);

                order.isOrder = true;
                order.shop_id = model.shopTo;
                id = await ordersRepo.AddCancellationAsync(order);
                orderDal = (await ordersRepo.GetCancellationsByShopIdInDateRange(order.shop_id, dtNow.AddSeconds(-1), dtNow)).Last(p => p.id == id);
                await strategy.UpdateAverageCost(Direction.Order, orderDal);
            }
            catch (Exception)
            {
                throw new Exception("Перемещение не удалось.");
            }
            
        }
    }
}
