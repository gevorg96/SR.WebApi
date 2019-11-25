using System;
using System.Linq;
using SmartRetail.App.DAL.DropBox;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Repository;
using Xunit;
using SmartRetail.App.Web.Models.Auth;
using System.Collections.Generic;
using System.Globalization;
using SmartRetail.App.Web.Models.Service;
using SmartRetail.App.Web.Models.Validation;
using System.Threading.Tasks;
using SmartRetail.App.DAL.Repository.Interfaces;
using SmartRetail.App.Web.Models.Interface;
using SmartRetail.App.Web.Models.ViewModel.Products;
using SmartRetail.App.DAL.BLL.Utils;
using SmartRetail.App.Web.Models.ViewModel.Sales;
using System.IO;
using SmartRetail.App.DAL.BLL.DataServices;

namespace SmartRetail.App.Test
{
    public class TestDAL
    {
        private const string conn =
            "Data Source=SQL6007.site4now.net;Initial Catalog=DB_A4E50E_smartretail;User Id=DB_A4E50E_smartretail_admin;Password=1234QWer;";

        private string postgresConn =
            "database=d369fmctn65rq9;host=ec2-46-137-91-216.eu-west-1.compute.amazonaws.com;password=f8774cccfae05a0a4c4bd86c0d40d504db71cd50ebaa52d5ad6767709ffcd5d4;username=nfedurzdslzhpo;port=5432;pooling=True;trust server certificate=True;ssl mode=Require";       
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

        public TestDAL()
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
        public void TestExpensesType()
        {
            var repo = new ExpensesTypeRepository(conn);
            var t = repo.GetAll();
        }

        [Fact]
        public void TestUserProfileRepo()
        {
            var repo = new UserRepository(conn);
            var shopRepo = new ShopRepository(conn);
            var businessRepo = new BusinessRepository(conn);
 
            /*var user1 = new UserProfile
            {
                UserName = "qwerty",
                Password = Crypto.HashPassword("55555"),
                shop_id = 1,
                business_id = 1,
                access_grade = 2
            };
            var user2 = new UserProfile
            {
                UserName = "admin@gmail.com",
                Password = Crypto.HashPassword("12345"),
                shop_id = 1,
                business_id = 1,
                access_grade = 2
            };
            var user3 = new UserProfile
            {
                UserName = "nikartash",
                Password = Crypto.HashPassword("1234qwer"),
                shop_id = null,
                business_id = 2,
                access_grade = 0
            };
            var user4 = new UserProfile
            {
                UserName = "sunzi",
                Password = Crypto.HashPassword("1234567890"),
                shop_id = null,
                business_id = 1,
                access_grade = 0
            };
            var lst = new List<UserProfile>
            {
                user3, user4
            };
            foreach (var user in lst)
            {
                repo.Add(user);
            }*/

            var user = new UserProfile
            {
                UserName = "gevkes",
                Password = Crypto.HashPassword("12345"),
                shop_id = 5,
                business_id = 2,
                access_grade = 1
            };
            repo.Add(user);

            //var login = "qwerty";
            //var user = repo.GetByLogin(login) as UserProfile;
            //if (user != null)
            //{
            //    repo.Update(user, "Password", Crypto.HashPassword(user.Password));
            //}

        }

        [Fact]
        public void TestProductRepo()
        {
            var repo = new ProductRepository(conn);


            var lst = new List<Product>
            {
                new Product
                {
                    //shop_id = 11,
                    business_id = 16,
                    name = "����������� �����",
                    attr1 = "����� (���������)"
                },
                new Product
                {
                    //shop_id = 11,
                    business_id = 16,
                    name = "����� �������",
                    attr1 = "����� (����������� ������������)"
                },
                new Product
                {
                    //shop_id = 11,
                    business_id = 16,
                    name = "������ �����",
                    attr1 = "����� ����������"
                },
                new Product
                {
                    //shop_id = 11,
                    business_id = 16,
                    name = "��������� ��������",
                    attr1 = "����� (����������� ������������)"
                }
            };

            foreach(var t in lst)
            {
                //repo.Add(t);
            }
        }

        [Fact]
        public void GetShopsByBusiness()
        {
            var shopRepo = new ShopRepository(postgresConn);
            var t = shopRepo.GetShopsByBusiness(1);
        }

        [Fact]
        public void TestExpensesRepo()
        {
            var expRepo = new ExpensesRepository(conn);
            //var t = expRepo.GetExpenses(1, 1, new DateTime(2019, 1, 1));
        }

        [Fact]
        public void TestAddProduct()
        {
            var repo = new ProductRepository(conn);
            var prod = new Product
            {
                //shop_id = 1,
                business_id = 1,
                name = "rewrew",
                unit_id = 1,
                attr9 = "52",
                attr10 = "градиент"
            };
            prod.Cost.Add(new Cost
            {
                value = (decimal?) 3213.3
            });
            prod.Prices.Add(new Price
            {
                price = 4200
            });
            prod.Stock.Add(new Stock
            {
                count = 40
            });

            repo.AddProduct(prod);
        }
        
        [Fact]
        public void AddShoesInProducts()
        {
            var prodrepo = new ProductRepository(conn);
            var product = new Product
            {
                business_id = 2,
                //shop_id = 4,
                attr1 = "пиво"
            };
            for (int i = 0; i < 1000; i++)
            {
                product.name = "пиво " + (i + 1).ToString(CultureInfo.InvariantCulture);
                product.attr9 = "500";
                prodrepo.AddProduct(product);
            }
        }

        [Fact]
        public async void AddStocks()
        {
            var stockRepo = new StockRepository(conn);
            var rnd = new Random();
            var s = new Stock
            {
                shop_id = 4,
            };
            for (int i = 141; i <= 150; i++)
            {
                s.prod_id = i;
                s.count = rnd.Next(1, 20);
                await stockRepo.AddAsync(s);
            }
        }

        [Fact]
        public void TestExpenses()
        {
            var expRepo = new ExpensesRepository(conn);
            var expenses = expRepo.GetExpensesAsync(1, null, new DateTime(2019, 3, 1), new DateTime(2019, 8, 1));
            //var groupExpenses = expenses.GroupBy(p => p.report_date).ToList();
        }

        [Fact]
        public void TestExpensesService()
        {
            var expService = new ExpensesService(new ExpensesRepository(conn), new ShopsChecker(new ShopRepository(conn)));
            var exps = expService.GetExpenses(new UserRepository(conn).GetById(10), null, new DateTime(2019, 3, 1),
                new DateTime(2019, 8, 1));
        }

        [Fact]
        public async void TestSmartUpdate()
        {
            var prodRepo = new ProductRepository(conn);
            var p = await prodRepo.GetByIdAsync(1133);
            p.attr1 = "пивас";
            p.attr2 = "разливуха";
            p.attr3 = "нефильтрованный";
            p.supplier_id = 2;
            prodRepo.UpdateProduct(p);
        }

        [Fact]
        public async void TestBusinessAsync()
        {
            var brepo = new BusinessRepository(conn);
            var b = await brepo.GetByIdAsync(3);
        }

        [Fact]
        public async Task TestAddShopAsync()
        {
            var shop = new Shop
            {
                business_id = 2,
                name = "Норм магаз",
                shop_address = "Ленина, 1"
            };
            var shopRepo = new ShopRepository(conn);
            await shopRepo.AddAsync(shop);
        }

        [Fact]
        public async void TestUpdateShopAsync()
        {
            var shopRepo = new ShopRepository(conn);
            var shop = shopRepo.GetById(6);
            shop.name = "Новый магазин";
            await shopRepo.UpdateAsync(shop);
        }

        [Fact]
        public async void TestAddProductAsync()
        {

            var prod = new ProductDetailViewModel
            {
                ProdName = "ewq3213213",
                VendorCode = "pic233123",
                Price = 2313,
                Cost = 1800,
                Color = "градиент",
                Size = "40",
                UnitId = 1,
                Category = "/products/1. Кайфы от Петерфельдо/Рыбалка/Спиннинги/Что-то",
                Stocks = new List<ShopStockViewModel>
                {
                    new ShopStockViewModel
                    {
                        ShopId = 1,
                        Stock = 5
                    },
                    new ShopStockViewModel
                    {
                        ShopId = 2,
                        Stock = 10
                    }
                },
                ImgBase64 = "/9j/4AAQSkZJRgABAQEAYABgAAD/4RD8RXhpZgAATU0AKgAAAAgABAE7AAIAAAAYAAAISodpAAQAAAABAAAIYpydAAEAAAAaAAAQ2uocAAcAAAgMAAAAPgAAAAAc6gAAAAgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAANCT0LXQstC+0YDQsyDQmtC10YHRj9C9AAAFkAMAAgAAABQAABCwkAQAAgAAABQAABDEkpEAAgAAAAMzMwAAkpIAAgAAAAMzMwAA6hwABwAACAwAAAikAAAAABzqAAAACAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAMjAxOTowNjoyNSAxMjowNTozMgAyMDE5OjA2OjI1IDEyOjA1OjMyAAAAEwQ1BDIEPgRABDMEIAAaBDUEQQRPBD0EAAD/4QsqaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wLwA8P3hwYWNrZXQgYmVnaW49J++7vycgaWQ9J1c1TTBNcENlaGlIenJlU3pOVGN6a2M5ZCc/Pg0KPHg6eG1wbWV0YSB4bWxuczp4PSJhZG9iZTpuczptZXRhLyI+PHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj48cmRmOkRlc2NyaXB0aW9uIHJkZjphYm91dD0idXVpZDpmYWY1YmRkNS1iYTNkLTExZGEtYWQzMS1kMzNkNzUxODJmMWIiIHhtbG5zOmRjPSJodHRwOi8vcHVybC5vcmcvZGMvZWxlbWVudHMvMS4xLyIvPjxyZGY6RGVzY3JpcHRpb24gcmRmOmFib3V0PSJ1dWlkOmZhZjViZGQ1LWJhM2QtMTFkYS1hZDMxLWQzM2Q3NTE4MmYxYiIgeG1sbnM6eG1wPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvIj48eG1wOkNyZWF0ZURhdGU+MjAxOS0wNi0yNVQxMjowNTozMi4zMzE8L3htcDpDcmVhdGVEYXRlPjwvcmRmOkRlc2NyaXB0aW9uPjxyZGY6RGVzY3JpcHRpb24gcmRmOmFib3V0PSJ1dWlkOmZhZjViZGQ1LWJhM2QtMTFkYS1hZDMxLWQzM2Q3NTE4MmYxYiIgeG1sbnM6ZGM9Imh0dHA6Ly9wdXJsLm9yZy9kYy9lbGVtZW50cy8xLjEvIj48ZGM6Y3JlYXRvcj48cmRmOlNlcSB4bWxuczpyZGY9Imh0dHA6Ly93d3cudzMub3JnLzE5OTkvMDIvMjItcmRmLXN5bnRheC1ucyMiPjxyZGY6bGk+0JPQtdCy0L7RgNCzINCa0LXRgdGP0L08L3JkZjpsaT48L3JkZjpTZXE+DQoJCQk8L2RjOmNyZWF0b3I+PC9yZGY6RGVzY3JpcHRpb24+PC9yZGY6UkRGPjwveDp4bXBtZXRhPg0KICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAg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"
            };

            await prodService.AddProduct(userRepo.GetById(5), prod);
        }

        [Fact]
        public async void TestAddProductAsyncV2()
        {
            var product = new Product
            {
                business_id = 1,
                unit_id = 1,
                name = "Тестовый товар",
                attr1 = "Атрибут1",
                attr9 = "43",
                attr10 = "синий"
            };
            var res = await productDataService.Insert(product);

        }

        [Fact]
        public async void TestUpdateProduct()
        {
            var product = new Product
            {
                id = 1273,
                business_id = 1,
                unit_id = 1,
                name = "Тестовый товар (изменённый)",
                attr1 = "Атрибут1",
                attr9 = "45",
                attr10 = "синий"
            };
            var res = await productDataService.Update(product);
        }

        [Fact]
        public async void TestPureOrderStocks()
        {
            var result = await orderStockRepo.GetPureOrderStocksByProdId(1158);
        }
        
        [Fact]
        public async void TestFifoStrategySales()
        {
            var model = new SalesCreateViewModel
            {
                shopId = 2,
                totalSum = 5876,
                reportDate = new DateTime(2019, 7, 20)
            };
            model.products = new List<SalesProductRowViewModel>
            {
                //new SalesProductRowViewModel
                //{
                //    count = 14,
                //    prodId = 1196,
                //    summ = 2313
                //},
                new SalesProductRowViewModel
                {
                    count = 2,
                    prodId = 1197,
                    summ = 250
                }
            };
            await salesService.AddSale(model);
        }
        
        [Fact]
        public async void TestGetBills()
        {
            var bills = await billsRepo.GetBillsWithSales(1, new DateTime(2019, 1, 1), new DateTime(2019, 8, 1));
        }

        [Fact]
        public async Task TestGetOrdersAsync()
        {
            var t = await ordersRepo.GetCancellationsByShopIdInDateRange(3, new DateTime(2019, 7, 14), new DateTime(2019, 7, 16));
        }

        [Fact]
        public async void FillExpenses()
        {
            var path = @"C:\Users\gevorg.kesyan\Desktop\1.rpt";

            var list = new List<ExpObject>();
            using (var sr = new StreamReader(path))
            {
                var str = sr.ReadLine();
                while (!string.IsNullOrEmpty(str))
                {
                    var line = str.Trim().Split(';');
                    var exp = new ExpObject
                    {
                        business_id = Convert.ToInt32(line[0]),
                        shop_id = Convert.ToInt32(line[1]),
                        expType = Convert.ToInt32(line[2]),
                        summ = Convert.ToDecimal(line[3]),
                        reportDate = DateTime.Parse(line[4])
                    };
                    list.Add(exp);
                    str = sr.ReadLine();
                }
            }

            var groups = list.GroupBy(p => p.business_id).ToList();
            foreach (var item in groups)
            {

                var innergroup = item.GroupBy(p => p.shop_id);
                foreach (var ig in innergroup)
                {
                    var iigroup = ig.GroupBy(p => p.reportDate);
                    foreach (var iig in iigroup)
                    {
                        var expenses = new Expense
                        {
                            business_id = item.Key,
                            shop_id = ig.Key,
                            report_date = iig.Key,
                            sum = iig.Sum(p => p.summ)
                        };
                        var iigList = iig.ToList();
                        foreach (var i in iigList)
                        {
                            expenses.ExpensesDetails.Add(new ExpensesDetail
                            {
                                expenses_type_id = i.expType,
                                sum = i.summ
                            });
                        }

                        await expRepo.AddExpenses(expenses);
                    }
                }
            }
        }

        [Fact]
        public async void TestSalesDataService()
        {
            var salesDs = new SalesDataService(prodRepo, imgRepo, billsRepo, businessRepo);
            var dailyData = await salesDs.GeTotalInfoAsync(1);
            var shares = await salesDs.GetSharesAsync(1);
            var pair = salesDs.GetTop2ProductsAsync(1);
        }

        [Fact]
        public async void TestExpensesAndStocksDataService()
        {
            var expensesDs = new ExpensesDataService(expRepo, new ExpensesTypeRepository(conn));
            var stocksDs = new StocksDataService(costRepo,stockRepo,imgRepo);

            //var exp = await expensesDs.GetMonthExpensesAsync(2);
            var stocks = await stocksDs.GetStocks(2);
        }

        [Fact]
        public async void TestFillFoldersByPath()
        {
            var folderDs = new FoldersDataService(foldersRepo,prodRepo);
            var t = await folderDs.GetFolderIdByPath("/Кайфы от Петерфельдо/Рыбалка/Что-то(2)/Что-то(3)", 1);
            //await folderDs.AddFoldersByPath("/Кайфы от Петерфельдо/Рыбалка/Что-то(4)/Что-то(5)", 1);
        }

        [Fact]
        public async void TestFillFoldersByRoot()
        {
            var t = await foldersRepo.GetSubTreeAsync(60);
        }

        [Fact]
        public async void TestGetPathById()
        {
            var t = await foldersRepo.GetPathByChildId(91);
        }
    }





    internal class ExpObject
    {
        public int business_id { get; set; }
        public int shop_id { get; set; }
        public int expType { get; set; }

        public decimal summ { get; set; }
        public DateTime reportDate { get; set; }
    }
}