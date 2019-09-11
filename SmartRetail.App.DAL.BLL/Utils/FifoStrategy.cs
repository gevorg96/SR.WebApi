using System;
using System.Linq;
using System.Threading.Tasks;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Helpers;
using SmartRetail.App.DAL.Repository;
using SmartRetail.App.DAL.Repository.Interfaces;

namespace SmartRetail.App.DAL.BLL.Utils
{
    public class FifoStrategy : IStrategy
    {
        private readonly IOrderStockRepository orderStockRepo;
        private readonly IStockRepository stockRepo;
        private readonly ICostRepository costRepo;

        public FifoStrategy(IOrderStockRepository orderStockRepository, 
            IStockRepository stockRepository, ICostRepository costRepository)
        {
            orderStockRepo = orderStockRepository;
            stockRepo = stockRepository;
            costRepo = costRepository;
        }

        public async Task UpdateAverageCost(Direction direction, IEntity entity)
        {
            switch (direction)
            {
                case Direction.Sale:
                    var sale = entity as Bills;
                    if (sale != null)
                        await SaleStrategy(sale);
                    break;
                case Direction.Order:
                    var order = entity as Orders;
                    if (order != null)
                        await OrderStrategy(order);
                    break;
                case Direction.Cancellation:
                    var cancel = entity as Orders;
                    if (cancel != null)
                        await CancellationStrategy(cancel);
                    break;
                default:
                    break;
            }
        }

        private async Task OrderStrategy(Orders order)
        {
            foreach (var orderDetail in order.OrderDetails)
            {
                var orderStock = new OrderStock
                {
                    order_id = orderDetail.id,
                    prod_id = orderDetail.prod_id,
                    curr_stocks = orderDetail.count,
                    shop_id = order.shop_id
                };
                await orderStockRepo.AddOrderStockAsync(orderStock);
                await UpdateCostAndStocks(orderDetail.prod_id, order.shop_id);
            }
        }

        private async Task SaleStrategy(Bills sale)
        {
            foreach (var item in sale.Sales)
            {
                await UpdateOrderStockBySales(item.count, item.prod_id, sale.shop_id);
                await UpdateCostAndStocks(item.prod_id, sale.shop_id);
            }
            
        }

        private async Task CancellationStrategy(Orders cancel)
        {
            foreach (var item in cancel.OrderDetails)
            {
                var count = item.count;
                await UpdateOrderStockBySales(count, item.prod_id, cancel.shop_id);
                await UpdateCostAndStocks(item.prod_id, cancel.shop_id);
            }
        }

        private async Task<decimal?> UpdateOrderStockBySales(decimal? salesCount, int prodId, int shopId)
        {
            var pureOrderStocks = await orderStockRepo.GetPureOrderStocksByProdAndShopIds(prodId, shopId);
            if (pureOrderStocks.Any())
            {
                var item = pureOrderStocks.FirstOrDefault();
                if (item != null)
                {
                    var temporary = salesCount - item.curr_stocks;
                    if (temporary <= 0)
                    {
                        item.curr_stocks -= salesCount;
                        await orderStockRepo.UpdateOrderStockAsync(item);
                        return temporary;
                    }
                    else
                    {
                        item.curr_stocks = 0;
                        await orderStockRepo.UpdateOrderStockAsync(item);
                        return await UpdateOrderStockBySales(temporary, prodId, shopId);
                    }
                }
            }
            throw new Exception("Нет элементов, для перерасчёта закупочной цены.");
        }

        private async Task UpdateCostAndStocks(int productId, int shopId)
        {
            var pureOrderStocks = await orderStockRepo.GetPureOrderStocksByProdId(productId);

            decimal? multipleSum = 0;
            decimal? count = 0;

            foreach (var item in pureOrderStocks)
            {
                multipleSum += item.curr_stocks * (item.OrderDetail != null ? item.OrderDetail.cost : 0);
                count += item.curr_stocks;
            }
            decimal? averageCost = 0;
            if (count != 0)
            {
                averageCost = multipleSum / count;

            }

            var cost = costRepo.GetByProdId(productId).FirstOrDefault();
            if (cost != null)
            {
                cost.value = averageCost;
                await costRepo.UpdateCostValueAsync(cost);
            }
            else
            {
                var c = new Cost
                {
                    prod_id = productId,
                    value = averageCost
                };
                await costRepo.AddCostAsync(c);
            }
            
            var stockDal = stockRepo.GetStockByShopAndProdIds(shopId, productId);
            var orderStockShop = await orderStockRepo.GetPureOrderStocksByProdAndShopIds(productId, shopId);
            if (stockDal != null)
            {
                stockDal.count = orderStockShop.Sum(p => p.curr_stocks);
                await stockRepo.UpdateValueAsync(stockDal);
            }
            else
            {
                var stock = new Stock
                {
                    prod_id = productId,
                    shop_id = shopId,
                    count = count
                };
                await stockRepo.AddAsync(stock);
            }
        }
    }
}
