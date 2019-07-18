using System;
using System.Linq;
using SmartRetail.App.DAL.DropBox;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Repository;
using SmartRetail.App.Web.Models;
using Xunit;
using SmartRetail.App.Web.Models.Auth;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using SmartRetail.App.Web.Models.Service;
using SmartRetail.App.Web.Models.Validation;

namespace SmartRetail.App.Test
{
    public class TestDAL
    {
        private const string conn =
            "Data Source=SQL6001.site4now.net;Initial Catalog=DB_A497DE_retailsys;User Id=DB_A497DE_retailsys_admin;Password=1234QWer;";  
        
        [Fact]
        public void TestBusiness()
        {
            var brepo = new BusinessRepository(conn);
            var all = brepo.GetAll();
        }
       
        [Fact]
        public void TestExpensesType()
        {
            var repo = new ExpensesTypeRepository(conn);
            var t = repo.GetAll();
        }

        [Fact]
        public void TestDailySales()
        {
            var dailyEntity = new MainService(conn);
            var daily = dailyEntity.GetDailyData(1);
            var month = dailyEntity.GetMonthData(1);
            var stocks = dailyEntity.GetStocks(2);
            var exData = dailyEntity.GetExpenses(0);
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
                    shop_id = 11,
                    business_id = 16,
                    name = "����������� �����",
                    attr1 = "����� (���������)"
                },
                new Product
                {
                    shop_id = 11,
                    business_id = 16,
                    name = "����� �������",
                    attr1 = "����� (����������� ������������)"
                },
                new Product
                {
                    shop_id = 11,
                    business_id = 16,
                    name = "������ �����",
                    attr1 = "����� ����������"
                },
                new Product
                {
                    shop_id = 11,
                    business_id = 16,
                    name = "��������� ��������",
                    attr1 = "����� (����������� ������������)"
                }
            };

            foreach(var t in lst)
            {
                repo.Add(t);
            }
        }

        [Fact]
        public void TestBusinessFilter()
        {
            
            var businessRepo = new BusinessRepository(conn);
            var t = businessRepo.GetWithFilter("user_profile_id", "5");
        }

        [Fact]
        public void GetShopsByBusiness()
        {
            var shopRepo = new ShopRepository(conn);
            var t = shopRepo.GetShopsByBusiness(3);
        }

        [Fact]
        public void GetShopsWithProducts()
        {
            var shopRepo = new ShopRepository(conn);
            var t = shopRepo.GetShopsByBusinessMultiMappingProducts(3);
        }

        [Fact]
        public void KuKU()
        {
            var businessRepo = new BusinessRepository(conn);
            var shopRepo = new ShopRepository(conn);
            var t = businessRepo.GetWithFilter("user_profile_id", 5.ToString()).FirstOrDefault();
            IEnumerable<Shop> shops = null;
            if (t != null)
            {
                shops = shopRepo.GetShopsByBusinessMultiMappingProducts(t.id);
            }

            var products = new List<Product>();
            if (shops != null && shops.Any())
            {
                foreach (var shop in shops)
                {
                    products.AddRange(shop.Product);
                }
            }
        }

        [Fact]
        public void TestSalesRepo()
        {
            var salesRepo = new SalesRepository(conn);
             //var t = salesRepo.GetSalesByShopAndReportDate(10, new DateTime(2019, 4, 1));
        }

        [Fact]
        public void TestPriceRepo()
        {
            var priceRepo = new PriceRepository(conn);
            var t = priceRepo.GetPricesByIds(new List<int> {2, 3, 4});
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
                shop_id = 1,
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
        public async void TestProductServiceGetFolders()
        {
            var prodservice = new ProductService(new ShopRepository(conn), new BusinessRepository(conn),
                new ImagesRepository(conn), new DropBoxBase("o9340xsv2mzn7ws", "xzky2fzfnmssik1"), 
                new ProductRepository(conn), new UnitRepository(conn), new PriceRepository(conn), 
                new ShopsChecker(new ShopRepository(conn), new BusinessRepository(conn)), new CostRepository(conn),new StockRepository(conn));
            var t = await prodservice.GetNexLevelGroup(new UserRepository(conn).GetById(5), null, false);

        }

        [Fact]
        public void AddShoesInProducts()
        {
            var prodrepo = new ProductRepository(conn);
            var product = new Product
            {
                business_id = 2,
                shop_id = 4,
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
        public void AddSales()
        {
            var prodrepo = new ProductRepository(conn);
            var salesRepo = new SalesRepository(conn);
            var products = prodrepo.GetProducts(37).ToList();
            var sale = new Sales
            {
                report_date = DateTime.Now,
                sales_count = 1,
                unit_id = 1
            };

            var b = 20;
            var rnd = new Random(2000);
            for (var i = 0; i < products.Count; i++)
            {
                var product = products[i];
                sale.prod_id = product.id;
                sale.shop_id = product.shop_id;
                sale.bill_number = i / 10 + b;
                sale.summ = (decimal?)rnd.NextDouble() + 2000;
                salesRepo.AddSales(sale);
            }
        }

        [Fact]
        public void AddStocks()
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
                stockRepo.Add(s);
            }
        }

        [Fact]
        public void TestStockDataService()
        {
            //var stockService = new StockService(new ShopRepository(conn), new BusinessRepository(conn), new StockRepository(conn));
            //var t = stockService.GetStocks(new UserRepository(conn).GetById(5), 1);
        }

        [Fact]
        public void TestExpenses()
        {
            var expRepo = new ExpensesRepository(conn);
            var expenses = expRepo.GetExpenses(1, null, new DateTime(2019, 3, 1), new DateTime(2019, 8, 1));
            var groupExpenses = expenses.GroupBy(p => p.report_date).ToList();
        }

        [Fact]
        public void TestExpensesService()
        {
            var expService = new ExpensesService(new ExpensesRepository(conn), new ShopsChecker(new ShopRepository(conn), new BusinessRepository(conn)));
            var exps = expService.GetExpenses(new UserRepository(conn).GetById(10), null, new DateTime(2019, 3, 1),
                new DateTime(2019, 8, 1));
        }

        [Fact]
        public void TestUpdateProducts()
        {
            var prodRepo = new ProductRepository(conn);
            var p = prodRepo.GetById(138);
            p.supplier_id = 1;
            prodRepo.UpdateProduct(p, "supplier_id");
        }

        [Fact]
        public void TestSmartUpdate()
        {
            var prodRepo = new ProductRepository(conn);
            var p = prodRepo.GetById(1133);
            p.attr1 = "пивас";
            p.attr2 = "разливуха";
            p.attr3 = "нефильтрованный";
            p.supplier_id = 2;
            prodRepo.UpdateProduct(p);
        }
        
        [Fact]
        public void DelegateProducts()
        {
            var prodRepo = new ProductRepository(conn);
            var products = prodRepo.GetAll().ToList();

            var s = 1;
            var b = 1;
            for (int i = 1; i <= products.Count(); i++)
            {
                var prod = products[i-1];
                if (i % 284 == 0)
                {
                    s++;
                }
                if (i % 568 == 0)
                {
                    b++;
                }
                prod.shop_id = s;
                prod.business_id = b;
                prodRepo.UpdateProduct(prod);
            }
        }

        [Fact]
        public async void TestBusinessAsync()
        {
            var brepo = new BusinessRepository(conn);
            var b = await brepo.GetByIdAsync(3);
        }

    }
}