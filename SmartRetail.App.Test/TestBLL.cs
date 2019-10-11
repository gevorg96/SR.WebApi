using System;
using Newtonsoft.Json.Bson;
using SmartRetail.App.DAL.BLL.DataServices;
using SmartRetail.App.DAL.BLL.Utils;
using SmartRetail.App.DAL.DropBox;
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
            await prodposStr.GetProductPositionOffDays(2,1194,new DateTime(2019, 5,15), new DateTime(2019, 9,18));
        }

        [Fact]
        public async void FillOrders()
        {
            
        }
        
    }
}