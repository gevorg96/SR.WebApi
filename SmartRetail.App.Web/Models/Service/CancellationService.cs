using SmartRetail.App.DAL.BLL.Utils;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Repository;
using SmartRetail.App.DAL.Repository.Interfaces;
using SmartRetail.App.Web.Models.Interface;
using SmartRetail.App.Web.Models.Validation;
using SmartRetail.App.Web.Models.ViewModel.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartRetail.App.Web.Models.Service
{
    public class CancellationService : ICancellationService
    {
        private readonly IOrderRepository orderRepo;
        private readonly IStrategy strategy;
        private readonly IShopRepository shopRepo;
        private readonly IProductRepository productRepo;
        private readonly IImageRepository imgRepo;
        private readonly ICostRepository costRepo;
        private ShopsChecker shopsChecker;

        public CancellationService(IUserRepository userRepository, IShopRepository shopRepository, IOrderRepository orderRepository, IProductRepository productRepository,
            IPriceRepository priceRepository, IImageRepository imageRepository, IStrategy _strategy, ICostRepository costRepository, ShopsChecker _shopsChecker)
        {
            imgRepo = imageRepository;
            shopRepo = shopRepository;
            orderRepo = orderRepository;
            productRepo = productRepository;
            shopsChecker = _shopsChecker;
            strategy = _strategy;
            costRepo = costRepository;
        }

        public async Task AddCancellations(OrderCreateViewModel model)
        {
            var cancels = model.products.Select(p => new Orders
            {
                prod_id = p.id,
                cost = 0,
                count = -p.count,
                report_date = model.reportDate,
                shop_id = model.shopId
            });

            foreach (var cancel in cancels)
            {
                await orderRepo.AddOrderAsync(cancel);
                var cancelDal = await orderRepo.GetLastCancellationAsync(cancel.shop_id, cancel.prod_id, model.reportDate.AddSeconds(-1), model.reportDate);
                await strategy.UpdateAverageCost(DAL.Helpers.Direction.Cancellation, cancelDal, cancelDal.prod_id, cancelDal.shop_id);
            }
        }

        public async Task<IEnumerable<OrderViewModel>> GetCancellations(UserProfile user, DateTime from, DateTime to, int shopId)
        {
            IEnumerable<Shop> shops = new List<Shop>();
            var orders = new List<OrderViewModel>();
            var ordersDal = new List<Orders>();

            var avl = shopsChecker.CheckAvailability(user, shopId);
            if (!avl.isCorrectShop)
                return new List<OrderViewModel>();
            if (!avl.hasShop && avl.isAdmin)
            {
                shops = shopRepo.GetShopsByBusiness(user.business_id.Value);
            }
            else if (!avl.hasShop && !avl.isAdmin)
            {
                return new List<OrderViewModel>();
            }
            else if (avl.hasShop)
            {
                shops = new List<Shop> { shopRepo.GetById(shopId) };
            }

            if (shops == null || !shops.Any())
                return new List<OrderViewModel>();

            foreach (var shop in shops)
            {
                ordersDal.AddRange(await orderRepo.GetCancellationsByShopId(shop.id, from, to));
            }

            var orderGroups = ordersDal.GroupBy(p => p.report_date).OrderByDescending(p => p.Key);
            foreach (var group in orderGroups)
            {
                var orderVm = new OrderViewModel
                {
                    reportDate = group.Key
                };
                foreach (var item in group)
                {
                    var prodDal = await productRepo.GetByIdAsync(item.prod_id);
                    var costDal = costRepo.GetByProdAndShopIds(item.prod_id, item.shop_id);
                    var prod = new OrderRowViewModel
                    {
                        id = item.id,
                        image = (await imgRepo.GetByIdAsync(item.prod_id)).img_url_temp,
                        name = prodDal.name,
                        price = costDal.value,
                        count = -item.count,
                        vendorCode = prodDal.attr1
                    };
                    prod.totalPrice = prod.price * prod.count;
                    orderVm.products.Add(prod);
                }
                orders.Add(orderVm);
            }
            return orders;
        }
    }
}
