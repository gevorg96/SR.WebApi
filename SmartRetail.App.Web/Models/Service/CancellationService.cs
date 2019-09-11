using SmartRetail.App.DAL.BLL.Utils;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Helpers;
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
        private readonly IOrdersRepository ordersRepo;
        private readonly IStrategy strategy;
        private readonly IShopRepository shopRepo;
        private readonly IProductRepository productRepo;
        private readonly IImageRepository imgRepo;
        private readonly ICostRepository costRepo;
        private ShopsChecker shopsChecker;

        public CancellationService(IUserRepository userRepository, IShopRepository shopRepository, IOrdersRepository ordersRepository, IProductRepository productRepository,
            IPriceRepository priceRepository, IImageRepository imageRepository, IStrategy _strategy, ICostRepository costRepository, ShopsChecker _shopsChecker)
        {
            imgRepo = imageRepository;
            shopRepo = shopRepository;
            ordersRepo = ordersRepository;
            productRepo = productRepository;
            shopsChecker = _shopsChecker;
            strategy = _strategy;
            costRepo = costRepository;
        }

        public async Task<OrderCreateViewModel> AddCancellations(OrderCreateViewModel model)
        {
           
            var order = new Orders
            {
                isOrder = false,
                report_date = model.reportDate,
                shop_id = model.shopId
            };

            order.OrderDetails = model.products.Select(p => new OrderDetails
            {
                prod_id = p.id,
                cost = costRepo.GetByProdId(p.id).FirstOrDefault() != null && costRepo.GetByProdId(p.id).FirstOrDefault().value.HasValue ?
                costRepo.GetByProdId(p.id).FirstOrDefault().value.Value : 0,
                count = p.count
            }).ToList();

            var id = 0;
            try
            {
                id = await ordersRepo.AddCancellationAsync(order);
                var orderDal = (await ordersRepo.GetCancellationsByShopIdInDateRange(order.shop_id, model.reportDate.AddSeconds(-1), model.reportDate)).Last(p => p.id == id);
                await strategy.UpdateAverageCost(Direction.Cancellation, orderDal);
            }
            catch (Exception)
            {

                throw new Exception("Добавление списания не удалось.");
            }
            
            model.id = id;
            return model;
        }

        public async Task<OrderViewModel> GetCancellation(UserProfile user, int id)
        {
            var cancel = await ordersRepo.GetByIdWithMultiAsync(id);
            if (cancel == null || cancel.isOrder)
            {
                return new OrderViewModel();
            }

            var vm = new OrderViewModel
            {
                id = cancel.id,
                reportDate = cancel.report_date
            };

            foreach (var item in cancel.OrderDetails)
            {
                var img = await imgRepo.GetByIdAsync(item.prod_id);
                var prod = await productRepo.GetByIdAsync(item.prod_id);
                vm.products.Add(new OrderRowViewModel
                {
                    vendorCode = prod.attr1,
                    name = prod.name,
                    count = item.count,
                    price = item.cost,
                    totalPrice = item.cost * item.count,
                    image = img != null && !string.IsNullOrEmpty(img.img_url_temp) ? img.img_url_temp : ""
                });
            }

            return vm;
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
                ordersDal.AddRange(await ordersRepo.GetCancellationsByShopIdInDateRange(shop.id, from, to));
            }

            var orderGroups = ordersDal.OrderByDescending(p => p.report_date);
            foreach (var group in orderGroups)
            {
                var orderVm = new OrderViewModel
                {
                    id = group.id,
                    reportDate = group.report_date
                };
                foreach (var item in group.OrderDetails)
                {
                    var prodDal = await productRepo.GetByIdAsync(item.prod_id);
                    var cost = costRepo.GetByProdAndShopIds(item.prod_id, shopId);
                    var prod = new OrderRowViewModel
                    {
                        image = (await imgRepo.GetByIdAsync(item.prod_id))?.img_url_temp,
                        name = prodDal.name,
                        price = cost.value,
                        count = item.count,
                        totalPrice = item.count * cost.value,
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
