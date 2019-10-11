using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dropbox.Api.Files;
using SmartRetail.App.DAL.Entities;
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
            var dict = new Dictionary<DateTime, KeyValuePair<List<Order>, List<Bill>>>();
            var stocksDict = new Dictionary<DateTime, decimal>();

            
            var orders =
                await _ordersRepository.GetOrdersByShopAndProdIdsInDateRange(shopId, productId, from, DateTime.Now);
            if (!orders.Any())
            {
                return 0;
            }

            var sales = await _billsRepository.GetBillsWithSalesByProdId(shopId, productId, from, DateTime.Now);
            var stock = _stockRepository.GetStockByShopAndProdIds(shopId, productId);

            for (var i = 0; i <= (DateTime.Now.Date - from.Date).Days; i++)
            {
                var dt = DateTime.Now.AddDays(-i).Date;
                var s = sales.Where(p => DayEquals(p.report_date.Date, dt)).ToList();
                var o = orders.Where(p => DayEquals(p.report_date.Date, dt)).ToList();
                dict[dt] = new KeyValuePair<List<Order>, List<Bill>>(o, s);
            }

            var stockValue = stock.count.Value;
            for (var i = 0; i < (DateTime.Now.Date - from.Date).Days; i++)
            {
                var dt = DateTime.Now.AddDays(-i).Date;
                var pair = dict[dt];
                stockValue = stockValue - pair.Value.Sum(p => p.Sales.Sum(q => q.count));
                stockValue = stockValue + pair.Key.Sum(p => p.OrderDetails.Sum(q => q.count));
            }

            for (int i = 0; i <= (to.Date - from.Date).Days; i++)
            {
                var dt = from.AddDays(i).Date;
                var pair = dict[dt];

                stockValue = stockValue + pair.Key.Sum(p => p.OrderDetails.Sum(q => q.count));
                stocksDict.Add(dt, stockValue);
                stockValue = stockValue - pair.Value.Sum(p => p.Sales.Sum(q => q.count));
            }


            return 0;
        }

        private bool DayEquals(DateTime first, DateTime second)
        {
            return first.Day == second.Day && first.Month == second.Month && first.Year == second.Year;
        }

        private Dictionary<DateTime, KeyValuePair<List<Order>, List<Bill>>> InitEmptyDictionary(DateTime from)
        {
            var dict = new Dictionary<DateTime, KeyValuePair<List<Order>, List<Bill>>>();
            for (var i = 0; i <= (DateTime.Now.Date - from.Date).Days; i++)
            {
                dict.Add(from.AddDays(i), new KeyValuePair<List<Order>, List<Bill>>());
            }

            return dict;
        }
    }
}