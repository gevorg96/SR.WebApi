using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;
using SmartRetail.App.DAL.Entities;

namespace SmartRetail.App.DAL.Repository
{
    public class ShopRepository: EntityRepository<Shop>, IShopRepository
    {
        private readonly string _connectionString;

        public ShopRepository(string conn)
        {
            _connectionString = conn;
        }

        public IEnumerable<Shop> GetWithFilter(string field, string value)
        {
            var sql = "SELECT * FROM Shop WHERE " + field + " = " + value;
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open();
                return db.Query<Shop>(sql).ToList();
            }
        }


        public IEnumerable<Shop> GetShopsByBusiness(int businessId)
        {
            string sql = "select * from Shop where business_id = @BusinessId";
            using (var connection = new SqlConnection(_connectionString))
            {
                return connection.Query<Shop>(sql, new {BusinessId = businessId});
            }
        }

        public IEnumerable<Shop> GetShopsByBusinessMultiMappingProducts(int businessId)
        {
            var sql = "select * from Shop where business_id = @BusinessId";
            var subSql = "select * from Product where shop_id = @shopId";
            var sub2Sql = "select * from Images where prod_id = @prodId";
            
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var shops = connection.Query<Shop>(sql, new { BusinessId = businessId });

                foreach (var shop in shops)
                {
                    shop.Product = connection.Query<Product>(subSql, new {shopId = shop.id}).ToList();
                    foreach (var product in shop.Product)
                        product.Image = connection.Query<Images>(sub2Sql, new {prodId = product.id}).FirstOrDefault();
                }

                return shops;
            }
        }
        
        public Shop GetShopMultiMappingProducts(int shopId)
        {
            var sql = "select * from Shop where id = @shopId";
            var subSql = "select * from Product where shop_id = @shopId";
            var sub2Sql = "select * from Images where prod_id = @prodId";
            
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var shop = connection.QueryFirst<Shop>(sql, new { shopId = shopId });

                shop.Product = connection.Query<Product>(subSql, new {shopId = shop.id}).ToList();
                foreach (var product in shop.Product)
                    product.Image = connection.Query<Images>(sub2Sql, new {prodId = product.id}).FirstOrDefault();
                

                return shop;
            }
        }

        public new Shop GetById(int shopId)
        {
            var sql = "SELECT * FROM Shop WHERE id = @ShopId";
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open();
                return db.Query<Shop>(sql, new {ShopId = shopId}).FirstOrDefault();
            }
        }
    }
}
