using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartRetail.App.DAL.BLL.Utils;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Repository.Interfaces;
using SmartRetail.App.Web.Models.Interface;
using SmartRetail.App.Web.Models.Validation;
using SmartRetail.App.Web.Models.ViewModel;
using SmartRetail.App.Web.Models.ViewModel.Sales;

namespace SmartRetail.App.Web.Models.Service
{
    public class SalesSerivce: ISalesService
    {
        private readonly IUserRepository _userRepo;
        private readonly IShopRepository _shopRepo;
        private readonly IBillsRepository _billsRepo;
        private readonly IProductRepository _productRepo;
        private readonly IPriceRepository _priceRepo;
        private readonly ICostRepository _costRepo;
        private readonly IImageRepository _imgRepo;
        private readonly IStrategy _strategy;
        private readonly ShopsChecker _shopsChecker;

        public SalesSerivce(IUserRepository userRepository, IShopRepository shopRepository, IBillsRepository billsRepository, IProductRepository productRepository,
            IPriceRepository priceRepository, IImageRepository imageRepository, IStrategy strategy, ShopsChecker shopsChecker, ICostRepository costRepo)
        {
            _imgRepo = imageRepository;
            _userRepo = userRepository;
            _shopRepo = shopRepository;
            _billsRepo = billsRepository;
            _productRepo = productRepository;
            _priceRepo = priceRepository;
            _costRepo = costRepo;
            _shopsChecker = shopsChecker;
            _strategy = strategy;
        }

        public async Task<int> AddSale(SalesCreateViewModel model)
        {
            var bill = new Bill
            {
                shop_id = model.shopId,
                sum = model.totalSum,
                report_date = model.reportDate
            };

            try
            {
                bill.Sales = (await Task.WhenAll(model.products.Select(async p => new Sale
                {
                    prod_id = p.prodId,
                    count = p.count,
                    sum = p.summ,
                    unit_id = (await _productRepo.GetByIdAsync(p.prodId))?.unit_id,
                    cost = _costRepo.GetByProdId(p.prodId).FirstOrDefault() != null && _costRepo.GetByProdId(p.prodId).FirstOrDefault().value.HasValue ?
                    _costRepo.GetByProdId(p.prodId).FirstOrDefault().value.Value : 0,
                    profit = p.summ - _costRepo.GetByProdId(p.prodId).FirstOrDefault().value.Value*p.count,
                    price = _priceRepo.GetPriceByProdId(p.prodId) != null && _priceRepo.GetPriceByProdId(p.prodId).price.HasValue ? _priceRepo.GetPriceByProdId(p.prodId).price.Value : 0
                }))).ToList();
            }
            catch (Exception)
            {
                throw new Exception("Не сделан ни один приход по одному из товаров чека.");
            }

            var billId = 0;
            try
            {
                billId = await _billsRepo.AddBillAsync(bill);
                await _strategy.UpdateAverageCost(DAL.Helpers.Direction.Sale, bill);
            }
            catch (Exception)
            {
                throw new Exception("Случилась ошибка при добавлении продажи.");
            }
            
            return billId;
        }

        public async Task<SalesViewModel> GetBill(UserProfile user, int billId)
        {
            var bill = await _billsRepo.GetByIdAsync(billId);
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

            var user = _userRepo.GetById(userId);

            var avl = _shopsChecker.CheckAvailability(user, shopId);
            if (!avl.isCorrectShop)
                return new List<SalesViewModel>();
            if (!avl.hasShop && avl.isAdmin)
            {
                shops = _shopRepo.GetShopsByBusiness(user.business_id.Value);
            }
            else if (!avl.hasShop && !avl.isAdmin)
            {
                return new List<SalesViewModel>();
            }
            else if (avl.hasShop)
            {
                shops = new List<Shop> { _shopRepo.GetById(shopId) };
            }

            if (shops == null || !shops.Any())
                return new List<SalesViewModel>();


            foreach (var shop in shops)
            {
                shop.Bills = (await _billsRepo.GetBillsWithSales(shop.id, from, to)).ToList();
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
                            imageUrl = (await _imgRepo.GetByIdAsync(sale.prod_id))?.img_url_temp,
                            ProdName = sale.Product.name,
                            VendorCode = "",
                            Summ = sale.sum,
                            Count = sale.count,
                            Price = sale.price
                            
                        });
                        if (sale.Product.Price != null && sale.Product.Price.price.HasValue)
                        {
                            totalSum += sale.price * sale.count;
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
