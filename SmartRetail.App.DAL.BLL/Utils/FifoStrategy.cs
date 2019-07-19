using System;
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

        public FifoStrategy(IProductRepository productRepository, IOrderStockRepository orderStockRepository, IStockRepository stockRepository, ICostRepository costRepository)
        {
            prodRepo = productRepository;
            orderStockRepo = orderStockRepository;
            stockRepo = stockRepository;
            costRepo = costRepository;
        }

        public async Task UpdateAverageCost(Direction direction, IEntity entity, int productId)
        {
            switch (direction)
            {
                case Direction.Sale:
                    var sale = entity as Sales;
                    if (entity != null)
                        await SaleStrategy(sale, productId);
                    break;
                case Direction.Order:
                    var order = entity as Orders;
                    if (entity != null)
                        await OrderStrategy(order, productId);
                    break;
                default:
                    break;
            }
        }

        private async Task OrderStrategy(Orders order, int productId)
        {
            var pureOrderStocks = await orderStockRepo.GetPureOrderStocksByProdId(productId);


        }
        private async Task SaleStrategy(Sales sale, int productId)
        {

        }
    }
}
