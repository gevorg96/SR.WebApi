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
        private readonly IProductRepository prodRepo;
        private readonly IOrderStockRepository orderStockRepo;
        private readonly IStockRepository stockRepo;
        private readonly ICostRepository costRepo;

        public FifoStrategy(IProductRepository productRepository, IOrderStockRepository orderStockRepository, 
            IStockRepository stockRepository, ICostRepository costRepository)
        {
            prodRepo = productRepository;
            orderStockRepo = orderStockRepository;
            stockRepo = stockRepository;
            costRepo = costRepository;
        }

        public async Task UpdateAverageCost(Direction direction, IEntity entity, int productId, int shopId)
        {
            switch (direction)
            {
                case Direction.Sale:
                    var sale = entity as Sales;
                    if (sale != null)
                        await SaleStrategy(sale, productId, shopId);
                    break;
                case Direction.Order:
                    var order = entity as Orders;
                    if (order != null)
                        await OrderStrategy(order, productId, shopId);
                    break;
                case Direction.Cancellation:
                    var cancel = entity as Orders;
                    if (cancel != null)
                        await CancellationStrategy(cancel, productId, shopId);
                    break;
                default:
                    break;
            }
        }

        private async Task OrderStrategy(Orders order, int productId, int shopId)
        {
            var orderStock = new OrderStock
            {
                order_id = order.id,
                prod_id = order.prod_id,
                curr_stocks = (decimal?)order.count
            };
            await orderStockRepo.AddOrderStockAsync(orderStock);

            await UpdateCostAndStocks(productId, shopId);
        }
        private async Task SaleStrategy(Sales sale, int productId, int shopId)
        {
            await UpdateOrderStockBySales(sale.sales_count, productId);
            await UpdateCostAndStocks(productId, shopId);
        }

        private async Task CancellationStrategy(Orders cancel, int productId, int shopId)
        {
            var count = -cancel.count;
            await UpdateOrderStockBySales(count, productId);
            await UpdateCostAndStocks(productId, shopId);
        }


        private async Task<decimal?> UpdateOrderStockBySales(decimal? salesCount, int prodId)
        {
            var pureOrderStocks = await orderStockRepo.GetPureOrderStocksByProdId(prodId);
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
                        return await UpdateOrderStockBySales(temporary, prodId);
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
                multipleSum += item.curr_stocks * (item.Order != null ? item.Order.cost : 0);
                count += item.curr_stocks;
            }
            var averageCost = multipleSum / count;

            var cost = costRepo.GetByProdId(productId).FirstOrDefault();
            cost.value = averageCost;

            await costRepo.UpdateCostValueAsync(cost);

            var stockDal = stockRepo.GetStockByShopAndProdIds(shopId, productId);
            if (stockDal != null)
            {
                stockDal.count = count;
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
