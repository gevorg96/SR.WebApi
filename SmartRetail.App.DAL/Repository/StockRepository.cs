using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using SmartRetail.App.DAL.Entities;
using static SmartRetail.App.DAL.Helpers.NullChecker;

namespace SmartRetail.App.DAL.Repository
{
    public class StockRepository: EntityRepository<Stock>, IStockRepository
    {
        private readonly string _connectionString;

        public StockRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<Stock> GetStocksWithProducts(int shopId)
        {
            var sql = "select * from Stock where shop_id = @shopId";
            var subSql = "select * from Product where id = @prodId";
            var sub2Sql = "select * from Images where prod_id = @prodId";
            
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var stocks = connection.Query<Stock>(sql, new { shopId = shopId }).ToList();

                foreach (var stock in stocks)
                {
                    stock.Product = connection.Query<Product>(subSql, new {prodId = stock.prod_id}).FirstOrDefault();
                    if (stock.Product != null)
                    {
                        stock.Product.Image = connection.Query<Images>(sub2Sql, new { prodId = stock.prod_id }).FirstOrDefault();
                    }
                }
                return stocks;
            }
        }

        public IEnumerable<Stock> GetStocksWithProductsByBusiness(int businessId)
        {
            var sql = "select * from Shop where business_id = @businessId";
            var subSql = "select * from Stock where shop_id = @shopId";
            var sub2Sql = "select * from Product where id = @prodId";
            var sub3Sql = "select * from Images where prod_id = @prodId";
            
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var shops = connection.Query<Shop>(sql, new {businessId = businessId});
                var stocks = new List<Stock>();
                foreach (var shop in shops)
                {
                    var stocksList = connection.Query<Stock>(subSql, new { shopId = shop.id }).ToList();
                    foreach (var stock in stocksList)
                    {
                        stock.Product = connection.Query<Product>(sub2Sql, new {prodId = stock.prod_id}).FirstOrDefault();
                        if (stock.Product != null)
                            stock.Product.Image = connection.Query<Images>(sub3Sql, new {prodId = stock.prod_id})
                                .FirstOrDefault();
                    }
                    stocks.AddRange(stocksList);
                    
                }
                return stocks;
            }
        }

        public new int Add(Stock entity)
        {
            var sql = "INSERT INTO Stock (shop_id, prod_id, count) values(" + isNotNull(entity.shop_id) + ", " +
                      entity.prod_id +
                      ", " + isNotNull(entity.count) + ")";
            var selectSql = "SELECT * FROM Stock WHERE prod_id = " + entity.prod_id;
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                conn.Execute(sql);
                var s = conn.Query<Stock>(selectSql).FirstOrDefault();
                if (s != null)
                {
                    return s.id;
                }

                return 0;
            }
        }

        public Stock GetStockByShopAndProdIds(int shopId, int prodId)
        {
            var sql = "select * from Stock where shop_id = " + shopId + " and prod_id = " + prodId;
            using (var db = new SqlConnection(_connectionString))
            {
                db.Open();
                return db.Query<Stock>(sql).FirstOrDefault();
            }
        }
    }
}