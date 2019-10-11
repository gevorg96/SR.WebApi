using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Bson;
using SmartRetail.App.DAL.BLL.DataServices;
using SmartRetail.App.DAL.BLL.Utils;
using SmartRetail.App.DAL.DropBox;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Repository;
using SmartRetail.App.DAL.Repository.Interfaces;
using SmartRetail.App.Web.Models.Interface;
using SmartRetail.App.Web.Models.Service;
using SmartRetail.App.Web.Models.Validation;
using Xunit;

namespace SmartRetail.App.Test
{
    public class TestBLL
    {
        private const string conn =
            "Data Source=SQL6007.site4now.net;Initial Catalog=DB_A4E50E_smartretail;User Id=DB_A4E50E_smartretail_admin;Password=1234QWer;";
        private const string dropboxBasePath = "/products";

        private readonly IShopRepository shopRepo;
        private readonly IBusinessRepository businessRepo;
        private readonly IImageRepository imgRepo;
        private readonly IPictureWareHouse dbBase;
        private readonly IProductRepository prodRepo;
        private readonly IUnitRepository unitRepo;
        private readonly IPriceRepository priceRepo;
        private readonly ICostRepository costRepo;
        private readonly IStockRepository stockRepo;
        private readonly IUserRepository userRepo;
        private readonly IOrdersRepository ordersRepo;
        private readonly IExpensesRepository expRepo;
        private readonly IBillsRepository billsRepo;
        private readonly IOrderStockRepository orderStockRepo;
        private readonly ShopsChecker checker;
        private readonly IProductService prodService;
        private readonly ISalesService salesService;
        private readonly IStrategy strategy;
        private readonly IFoldersRepository foldersRepo;
        private readonly ProductDataService productDataService;

        public TestBLL()
        {
            shopRepo = new ShopRepository(conn);
            businessRepo = new BusinessRepository(conn);
            imgRepo = new ImagesRepository(conn);
            prodRepo = new ProductRepository(conn);
            unitRepo = new UnitRepository(conn);
            priceRepo = new PriceRepository(conn);
            costRepo = new CostRepository(conn);
            stockRepo = new StockRepository(conn);
            userRepo = new UserRepository(conn);
            billsRepo = new BillsRepository(conn);
            expRepo = new ExpensesRepository(conn);
            foldersRepo = new FoldersRepository(conn);
            dbBase = new DropBoxBase("o9340xsv2mzn7ws", "xzky2fzfnmssik1");
            checker = new ShopsChecker(shopRepo);
            dbBase.GeneratedAuthenticationURL();
            dbBase.GenerateAccessToken();
            prodService = new ProductService(shopRepo, businessRepo, imgRepo, dbBase, prodRepo, unitRepo, priceRepo, 
                costRepo, stockRepo,ordersRepo, strategy, new FoldersDataService(foldersRepo,prodRepo));
            orderStockRepo = new OrderStockRepository(conn);
            strategy = new FifoStrategy(orderStockRepo, stockRepo, costRepo);
            salesService = new SalesSerivce(userRepo, shopRepo, billsRepo, prodRepo, priceRepo, imgRepo, strategy, checker, costRepo);
            ordersRepo = new OrdersRepository(conn);
            productDataService = new ProductDataService(dbBase);
        }

        [Fact]
        public async void TestProductPositionDayOff()
        {
            var prodposStr = new ProductPositionStrategy(shopRepo,prodRepo,stockRepo,ordersRepo,billsRepo);
            await prodposStr.GetProductPositionOffDays(2,1194,new DateTime(2019, 9,15), new DateTime(2019, 9,20));
        }

        [Fact]
        public async void FillOrders()
        {
            var orderDs = new OrderDataService();

            var rnd = new Random();
            var dt = new DateTime(2019, 7,5);
            while (true)
            {
                dt = dt.AddDays(rnd.Next(3, 8));
                if (dt >= DateTime.Now)
                    break;
                
                var order = new Order
                {
                    isOrder = true,
                    report_date = dt,
                    shop_id = 2,
                    OrderDetails = new List<OrderDetail>
                    {
                        new OrderDetail {prod_id = 1194, cost = rnd.Next(800, 1200), count = rnd.Next(2,8)},
                        new OrderDetail {prod_id = 1201, cost = rnd.Next(1750, 2150), count = rnd.Next(2,5)},
                        new OrderDetail {prod_id = 1212, cost = rnd.Next(2350, 2850), count = rnd.Next(2, 5)}
                    }
                };

                var id = await orderDs.Insert(order);
            }
            
        }

        [Fact]
        public async void FillSales()
        {
            var billDs = new BillDataService();
            var rnd = new Random();
            var dt = new DateTime(2019, 8,6);
            
            var prodList = new List<int>{1194};
            
            while (true)
            {
                dt = dt.AddDays(rnd.Next(2,4));
                if (dt >= new DateTime(2019, 9,20))
                    break;

                var bill = new Bill
                {
                    shop_id = 2,
                    report_date = dt,
                    sum = 0
                };
                
                var list = new List<Sale>();
                foreach (var p in prodList)
                {
                    var cost = costRepo.GetByProdId(p).FirstOrDefault();
                    var price = priceRepo.GetPriceByProdId(p);
                    var sale = new Sale
                    {
                        prod_id = p,
                        count = rnd.Next(1,3),
                        unit_id = (await prodRepo.GetByIdAsync(p))?.unit_id,
                        cost = cost?.value ?? 0,
                        price = price?.price ?? 0
                    };
                    sale.sum = price?.price != null
                        ? (price.price.Value + rnd.Next(-100, 50)) * sale.count
                        : (cost.value.Value + rnd.Next(500, 1000)) * sale.count;
                    
                    sale.profit = sale.sum - (sale.cost != 0 ? sale.cost * sale.count : sale.sum);
                    list.Add(sale);
                }

                bill.Sales = list;
                bill.sum = bill.Sales.Sum(p => p.sum) + rnd.Next(-200, 50);
                    
                

                var id = await billDs.Insert(bill);
            }
            
        } 
    }
}