using System;
using System.Linq;
using System.Threading.Tasks;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Helpers;
using SmartRetail.App.DAL.Repository;
using SmartRetail.App.DAL.Repository.Interfaces;
using SmartRetail.App.DAL.UnitOfWork;

namespace SmartRetail.App.DAL.BLL.Utils
{
    public class FifoStrategy : IStrategy
    {
        private IOrderStockRepository _orderStockRepo;
        private IStockRepository _stockRepo;
        private ICostRepository _costRepo;

        public FifoStrategy(IOrderStockRepository orderStockRepository, 
            IStockRepository stockRepository, ICostRepository costRepository)
        {
            _orderStockRepo = orderStockRepository;
            _stockRepo = stockRepository;
            _costRepo = costRepository;
        }

        public FifoStrategy()
        { }

        

        public async Task UpdateAverageCostUow(Direction direction, IEntity entity, IUnitOfWork uow)
        {
            switch (direction)
            {
                case Direction.Sale:
                    if (entity is BillParent sale)
                        await SalesStrategyUow(sale, uow);
                    break;
                case Direction.Order:
                    if (entity is Order order)
                        await OrderStrategyUow(order, uow);
                    break;
                case Direction.Cancellation:
                    if (entity is Order cancel)
                        await CancellationStrategyUow(cancel, uow);
                    break;
            }
        }

        private async Task OrderStrategyUow(Order order, IUnitOfWork uow)
        {
            _orderStockRepo = new OrderStockRepository(uow);

            foreach (var orderDetail in order.OrderDetails)
            {
                var orderStock = new OrderStock
                {
                    order_id = orderDetail.id,
                    prod_id = orderDetail.prod_id,
                    curr_stocks = orderDetail.count,
                    shop_id = order.shop_id
                };
                await _orderStockRepo.InsertUow(orderStock);
                await UpdateCostAndStocksUow(orderDetail.prod_id, order.shop_id, uow);
            }
        }

        private async Task SalesStrategyUow(BillParent bill, IUnitOfWork uow)
        {
            _orderStockRepo = new OrderStockRepository(uow);

            foreach (var item in bill.Sales)
            {
                await UpdateOrderStockBySalesUow(item.count, item.prod_id, bill.shop_id);
                await UpdateCostAndStocksUow(item.prod_id, bill.shop_id, uow);
            }
        }

        private async Task CancellationStrategyUow(Order cancel, IUnitOfWork uow)
        {
            _orderStockRepo = new OrderStockRepository(uow);
            foreach (var item in cancel.OrderDetails)
            {
                var count = item.count;
                await UpdateOrderStockBySalesUow(count, item.prod_id, cancel.shop_id);
                await UpdateCostAndStocksUow(item.prod_id, cancel.shop_id, uow);
            }
        }

        private async Task<decimal?> UpdateOrderStockBySalesUow(decimal? salesCount, int prodId, int shopId)
        {
            var pureOrderStocks = await _orderStockRepo.GetPureOrderStocksByProdAndShopIdsUow(prodId, shopId);
            if (pureOrderStocks.Any())
            {
                var item = pureOrderStocks.FirstOrDefault();
                if (item != null)
                {
                    var temporary = salesCount - item.curr_stocks;
                    if (temporary <= 0)
                    {
                        item.curr_stocks -= salesCount;
                        await _orderStockRepo.UpdateUow(item);
                        return temporary;
                    }
                    else
                    {
                        item.curr_stocks = 0;
                        await _orderStockRepo.UpdateUow(item);
                        return await UpdateOrderStockBySalesUow(temporary, prodId, shopId);
                    }
                }
            }
            throw new Exception("Нет элементов, для перерасчёта закупочной цены.");
        }

        private async Task UpdateCostAndStocksUow(int productId, int shopId, IUnitOfWork uow)
        {
            _costRepo = new CostRepository(uow);
            _stockRepo = new StockRepository(uow);
            var pureOrderStocks = await _orderStockRepo.GetPureOrderStocksByProdIdUow(productId);

            decimal? multipleSum = 0;
            decimal? count = 0;

            foreach (var item in pureOrderStocks)
            {
                multipleSum += item.curr_stocks * (item.OrderDetail?.cost ?? 0);
                count += item.curr_stocks;
            }
            decimal? averageCost = 0;
            if (count != 0)
            {
                averageCost = multipleSum / count;

            }

            var cost = (await _costRepo.GetByProdIdUow(productId)).FirstOrDefault();
            if (cost != null)
            {
                cost.value = averageCost;
                await _costRepo.UpdateUow(cost);
            }
            else
            {
                var c = new Cost
                {
                    prod_id = productId,
                    value = averageCost
                };
                await _costRepo.InsertUow(c);
            }

            var stockDal = await _stockRepo.GetStockByShopAndProdIdsUow(shopId, productId);
            var orderStockShop = await _orderStockRepo.GetPureOrderStocksByProdAndShopIdsUow(productId, shopId);
            if (stockDal != null)
            {
                stockDal.count = orderStockShop.Sum(p => p.curr_stocks);
                await _stockRepo.UpdateUow(stockDal);
            }
            else
            {
                var stock = new Stock
                {
                    prod_id = productId,
                    shop_id = shopId,
                    count = count
                };
                await _stockRepo.InsertUow(stock);
            }
        }


        #region Without Transactions

        public async Task UpdateAverageCost(Direction direction, IEntity entity)
        {
            switch (direction)
            {
                case Direction.Sale:
                    var sale = entity as BillParent;
                    if (sale != null)
                        await SaleStrategy(sale);
                    break;
                case Direction.Order:
                    var order = entity as Order;
                    if (order != null)
                        await OrderStrategy(order);
                    break;
                case Direction.Cancellation:
                    var cancel = entity as Order;
                    if (cancel != null)
                        await CancellationStrategy(cancel);
                    break;
                default:
                    break;
            }
        }

        private async Task OrderStrategy(Order order)
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
                await _orderStockRepo.AddOrderStockAsync(orderStock);
                await UpdateCostAndStocks(orderDetail.prod_id, order.shop_id);
            }
        }

        private async Task SaleStrategy(BillParent sale)
        {
            foreach (var item in sale.Sales)
            {
                await UpdateOrderStockBySales(item.count, item.prod_id, sale.shop_id);
                await UpdateCostAndStocks(item.prod_id, sale.shop_id);
            }
        }

        private async Task CancellationStrategy(Order cancel)
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
            var pureOrderStocks = await _orderStockRepo.GetPureOrderStocksByProdAndShopIds(prodId, shopId);
            if (pureOrderStocks.Any())
            {
                var item = pureOrderStocks.FirstOrDefault();
                if (item != null)
                {
                    var temporary = salesCount - item.curr_stocks;
                    if (temporary <= 0)
                    {
                        item.curr_stocks -= salesCount;
                        await _orderStockRepo.UpdateOrderStockAsync(item);
                        return temporary;
                    }
                    else
                    {
                        item.curr_stocks = 0;
                        await _orderStockRepo.UpdateOrderStockAsync(item);
                        return await UpdateOrderStockBySales(temporary, prodId, shopId);
                    }
                }
            }
            throw new Exception("Нет элементов, для перерасчёта закупочной цены.");
        }

        private async Task UpdateCostAndStocks(int productId, int shopId)
        {
            var pureOrderStocks = await _orderStockRepo.GetPureOrderStocksByProdId(productId);

            decimal? multipleSum = 0;
            decimal? count = 0;

            foreach (var item in pureOrderStocks)
            {
                multipleSum += item.curr_stocks * (item.OrderDetail?.cost ?? 0);
                count += item.curr_stocks;
            }
            decimal? averageCost = 0;
            if (count != 0)
            {
                averageCost = multipleSum / count;

            }

            var cost = _costRepo.GetByProdId(productId).FirstOrDefault();
            if (cost != null)
            {
                cost.value = averageCost;
                await _costRepo.UpdateCostValueAsync(cost);
            }
            else
            {
                var c = new Cost
                {
                    prod_id = productId,
                    value = averageCost
                };
                await _costRepo.AddCostAsync(c);
            }

            var stockDal = _stockRepo.GetStockByShopAndProdIds(shopId, productId);
            var orderStockShop = await _orderStockRepo.GetPureOrderStocksByProdAndShopIds(productId, shopId);
            if (stockDal != null)
            {
                stockDal.count = orderStockShop.Sum(p => p.curr_stocks);
                await _stockRepo.UpdateValueAsync(stockDal);
            }
            else
            {
                var stock = new Stock
                {
                    prod_id = productId,
                    shop_id = shopId,
                    count = count
                };
                await _stockRepo.AddAsync(stock);
            }
        }

        #endregion
    }
}
