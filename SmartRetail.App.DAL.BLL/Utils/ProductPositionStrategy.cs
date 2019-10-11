using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartRetail.App.DAL.Repository.Interfaces;

namespace SmartRetail.App.DAL.BLL.Utils
{
    public class ProductPositionStrategy: IProductPositionStrategy
    {
        private IShopRepository _shopRepository;
        private IProductRepository _productRepository;
        private IStockRepository _stockRepository;
        private IOrdersRepository _ordersRepository;
        private IBillsRepository _billsRepository;

        public ProductPositionStrategy(IShopRepository shopRepository, IProductRepository productRepository, IStockRepository stockRepository, IOrdersRepository ordersRepository, IBillsRepository billsRepository)
        {
            _shopRepository = shopRepository;
            _productRepository = productRepository;
            _stockRepository = stockRepository;
            _ordersRepository = ordersRepository;
            _billsRepository = billsRepository;
        }

        public async Task<double> GetProductPositionOffDays(int shopId, int productId, DateTime from, DateTime to)
        {
            var dict = new Dictionary<DateTime, bool>();
            var period = (to.Date - from.Date).Days;

            var orders = await _ordersRepository.GetOrdersByShopAndProdIdsInDateRange(shopId, productId, from, DateTime.Now);
            

            return 0;
        }
    }
}