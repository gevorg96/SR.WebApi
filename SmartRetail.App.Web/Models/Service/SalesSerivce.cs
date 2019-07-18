using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Repository;
using SmartRetail.App.Web.Models.Interface;
using SmartRetail.App.Web.Models.Validation;
using SmartRetail.App.Web.Models.ViewModel;

namespace SmartRetail.App.Web.Models.Service
{
    public class SalesSerivce: ISalesService
    {
        private readonly IUserRepository userRepo;
        private readonly IShopRepository shopRepo;
        private readonly ISalesRepository salesRepo;
        private readonly IProductRepository productRepo;
        private readonly IPriceRepository priceRepo;
        private readonly IImageRepository imgRepo;
        private ShopsChecker shopsChecker;

        public SalesSerivce(string conn)
        {
            imgRepo = new ImagesRepository(conn);
            userRepo = new UserRepository(conn);
            shopRepo = new ShopRepository(conn);
            salesRepo = new SalesRepository(conn);
            productRepo = new ProductRepository(conn);
            priceRepo = new PriceRepository(conn);
            shopsChecker = new ShopsChecker(shopRepo, new BusinessRepository(conn));
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
                shop.Sales = salesRepo.GetSalesByShopAndReportDate(shop.id, from, to).ToList();
            }

            foreach (var shop in shops)
            {
                var sales = shop.Sales;

                // группируем по чекам
                var groupedSales = sales.GroupBy(p => p.bill_number).ToList();
                foreach (var sale in groupedSales)
                {
                    decimal totalSum = 0;
                    decimal totalSumFirst = 0;
                    decimal discount = 0;
                    
                    var productsVm = new List<SalesProductViewModel>();
                    var products = new List<Product>();
                    
                    // итерируемся по продажам в чеке
                    foreach (var s in sale)
                    {
                        totalSum += s.summ ?? 0;
                        var prod = await productRepo.GetByIdAsync(s.prod_id);

                        // заполняем информацию по продукту
                        productsVm.Add(new SalesProductViewModel
                        {
                            imageUrl = imgRepo.GetById(s.prod_id)?.img_url_temp,
                            ProdName = s.Product.name,
                            VendorCode = "",
                            Summ = s.summ ?? 0,
                            Count = s.sales_count ?? 0,
                            Price = s.Product?.Price?.price
                            //Price = s.summ.HasValue && s.sales_count.HasValue ? (decimal)(s.summ/s.sales_count) : 0
                        });
                        products.Add(prod);
                        
                        // вытаскиваем цену
                        var price = priceRepo.GetPriceByProdId(s.prod_id);
                        if (price?.price == null) continue;
                        if (s.sales_count != null)
                            totalSumFirst += price.price.Value * s.sales_count.Value;
                    }

                    discount = totalSumFirst != 0 ? Math.Ceiling((1 - totalSum / totalSumFirst) * 100) : 0;
                    
                    var saleVm = new SalesViewModel
                    {   
                        billNumber = sale.Key.Value,
                        reportDate = sale.First().report_date,
                        products = productsVm,
                        totalSum = totalSum,
                        discount = discount < 0 ? 0 : discount
                    };
                    salesVm.Add(saleVm);

                }
            }
            return salesVm;
        }
    }
}
