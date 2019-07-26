using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartRetail.App.DAL.BLL.Utils;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Repository;
using SmartRetail.App.DAL.Repository.Interfaces;
using SmartRetail.App.Web.Models.Interface;
using SmartRetail.App.Web.Models.Validation;
using SmartRetail.App.Web.Models.ViewModel;
using SmartRetail.App.Web.Models.ViewModel.Sales;

namespace SmartRetail.App.Web.Models.Service
{
    public class SalesSerivce: ISalesService
    {
        private readonly IUserRepository userRepo;
        private readonly IShopRepository shopRepo;
        private readonly IBillsRepository billsRepo;
        private readonly ISalesRepository salesRepo;
        private readonly IProductRepository productRepo;
        private readonly IPriceRepository priceRepo;
        private readonly IImageRepository imgRepo;
        private readonly IStrategy strategy;
        private ShopsChecker shopsChecker;

        public SalesSerivce(IUserRepository userRepository, IShopRepository shopRepository, IBillsRepository billsRepository, ISalesRepository salesRepository, IProductRepository productRepository,
            IPriceRepository priceRepository, IImageRepository imageRepository, IStrategy _strategy, ShopsChecker _shopsChecker)
        {
            imgRepo = imageRepository;
            userRepo = userRepository;
            shopRepo = shopRepository;
            billsRepo = billsRepository;
            salesRepo = salesRepository;
            productRepo = productRepository;
            priceRepo = priceRepository;
            shopsChecker = _shopsChecker;
            strategy = _strategy;
        }

        public async Task<int> AddSale(SalesCreateViewModel model)
        {
            var bill = new Bills
            {
                shop_id = model.shopId,
                sum = model.totalSum,
                report_date = model.reportDate
            };
            bill.Sales = (await Task.WhenAll(model.products.Select(async p => new Sales
            {
                prod_id = p.prodId,
                count = p.count,
                sum = p.summ,
                unit_id = (await productRepo.GetByIdAsync(p.prodId)).unit_id
            }))).ToList();

            var billId = await billsRepo.AddBillAsync(bill);
            await strategy.UpdateAverageCost(DAL.Helpers.Direction.Sale, bill);

            return billId;
        }

        public async Task<SalesViewModel> GetBill(UserProfile user, int billId)
        {
            var bill = await billsRepo.GetByIdAsync(billId);
            if (bill == null)
            {
                return new SalesViewModel();
            }
            var sales = await GetSales(user.UserId, bill.shop_id, bill.report_date.AddSeconds(-1), bill.report_date.AddSeconds(1));
            return sales.FirstOrDefault(p => p.id == billId);
        }

        public async Task<IEnumerable<SalesViewModel>> GetSales(int userId, int shopId, DateTime from, DateTime to)
        {
            IEnumerable<Shop> shops = new List<Shop>();
            var salesVm = new List<SalesViewModel>();

            var user = userRepo.GetById(userId);

            var avl = shopsChecker.CheckAvailability(user, shopId);
            if (!avl.isCorrectShop)
                return new List<SalesViewModel>();
            if (!avl.hasShop && avl.isAdmin)
            {
                shops = shopRepo.GetShopsByBusiness(user.business_id.Value);
            }
            else if (!avl.hasShop && !avl.isAdmin)
            {
                return new List<SalesViewModel>();
            }
            else if (avl.hasShop)
            {
                shops = new List<Shop> { shopRepo.GetById(shopId) };
            }

            if (shops == null || !shops.Any())
                return new List<SalesViewModel>();


            foreach (var shop in shops)
            {
                shop.Bills = (await billsRepo.GetBillsWithSales(shop.id, from, to)).ToList();
            }

            foreach (var shop in shops)
            {
                var bills = shop.Bills;

                foreach (var bill in bills)
                {
                    decimal totalSum = 0;
                    
                    decimal discount = 0;

                    var productsVm = new List<SalesProductViewModel>();
                    var products = new List<Product>();

                    foreach (var sale in bill.Sales)
                    {
                        productsVm.Add(new SalesProductViewModel
                        {
                            imageUrl = (await imgRepo.GetByIdAsync(sale.prod_id))?.img_url_temp,
                            ProdName = sale.Product.name,
                            VendorCode = "",
                            Summ = sale.sum*sale.count,
                            Count = sale.count,
                            Price = sale.Product?.Price?.price
                            
                        });
                        if (sale.Product.Price != null && sale.Product.Price.price.HasValue)
                        {
                            totalSum += sale.Product.Price.price.Value * sale.count;
                        }
                    }

                    discount = totalSum != 0 ? Math.Ceiling((1 - bill.sum / totalSum) * 100) : 0;
                    var saleVm = new SalesViewModel
                    {
                        id = bill.id,
                        reportDate = bill.report_date,
                        products = productsVm,
                        totalSum = bill.sum,
                        discount = discount < 0 ? 0 : discount
                    };
                    salesVm.Add(saleVm);
                }

                // группируем по чекам
                //var groupedSales = sales.GroupBy(p => p.bill_number).ToList();
                //foreach (var sale in groupedSales)
                //{
                //    decimal totalSum = 0;
                //    decimal totalSumFirst = 0;
                //    decimal discount = 0;

                //    var productsVm = new List<SalesProductViewModel>();
                //    var products = new List<Product>();

                //    // итерируемся по продажам в чеке
                //    foreach (var s in sale)
                //    {
                //        totalSum += s.summ ?? 0;
                //        var prod = await productRepo.GetByIdAsync(s.prod_id);

                //        // заполняем информацию по продукту
                //        productsVm.Add(new SalesProductViewModel
                //        {
                //            imageUrl = (await imgRepo.GetByIdAsync(s.prod_id))?.img_url_temp,
                //            ProdName = s.Product.name,
                //            VendorCode = "",
                //            Summ = s.summ ?? 0,
                //            Count = s.sales_count ?? 0,
                //            Price = s.Product?.Price?.price
                //            //Price = s.summ.HasValue && s.sales_count.HasValue ? (decimal)(s.summ/s.sales_count) : 0
                //        });
                //        products.Add(prod);

                //        // вытаскиваем цену
                //        var price = priceRepo.GetPriceByProdId(s.prod_id);
                //        if (price?.price == null) continue;
                //        if (s.sales_count != null)
                //            totalSumFirst += price.price.Value * s.sales_count.Value;
                //    }

                //    discount = totalSumFirst != 0 ? Math.Ceiling((1 - totalSum / totalSumFirst) * 100) : 0;

                //    var saleVm = new SalesViewModel
                //    {   
                //        id = sale.Key.Value,
                //        reportDate = sale.First().report_date,
                //        products = productsVm,
                //        totalSum = totalSum,
                //        discount = discount < 0 ? 0 : discount
                //    };
                //    salesVm.Add(saleVm);

                //}
            }
            return salesVm;
        }
    }
}
