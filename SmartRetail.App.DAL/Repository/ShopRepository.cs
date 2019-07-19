using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Helpers;
using static SmartRetail.App.DAL.Helpers.NullChecker;

namespace SmartRetail.App.DAL.Repository
{
    public class ShopRepository: IShopRepository
    {
        private readonly string _connectionString;
        private QueryBuilder qb;

        public ShopRepository(string conn)
        {
            _connectionString = conn;
            qb = new QueryBuilder();
        }

        #region Read

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

        public Shop GetById(int shopId)
        {
            var sql = "SELECT * FROM Shop WHERE id = @ShopId";
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open();
                return db.Query<Shop>(sql, new {ShopId = shopId}).FirstOrDefault();
            }
        }

        #endregion

        #region Create

        public async Task AddAsync(Shop entity)
        {
            var sql = "insert into Shop (business_id, shop_address, name) values (" + isNotNull(entity.business_id) + 
                ", " + isNotNull(entity.shop_address) + ", " + isNotNull(entity.name) + ");";
            using(var db = new SqlConnection(_connectionString))
            {
                db.Open();
                var affRows = await db.ExecuteAsync(sql);
            }
        }

        #endregion

        #region Update

        public async Task UpdateAsync(Shop entity)
        {
            qb.Clear();
            var select = qb.Select("*").From("Shop").Where("id").Op(Ops.Equals, entity.id.ToString());
            var sb = new StringBuilder();
            sb.Append("update Shop set ");

            var pi = entity.GetType().GetProperties();

            using (var db = new SqlConnection(_connectionString))
            {
                db.Open();
                var row = db.QueryFirstOrDefault<Shop>(select.ToString());
                for (var i = 1; i < 4; i++)
                {
                    var p = pi[i];
                    var pt = p.PropertyType.ToString();
                    var newValue = p.GetValue(entity);
                    var oldValue = p.GetValue(row);
                    if (newValue != null && (oldValue == null || newValue.ToString() != oldValue.ToString()))
                    {
                        sb.Append(p.Name + " = " + QueryHelper.GetSqlString(p, p.GetValue(entity)) + ", ");
                    }
                }
                if (sb.Length < 20) return;

                sb.Remove(sb.Length - 2, 2);
                sb.Append(" where id = " + entity.id);
                var rows = await db.ExecuteAsync(sb.ToString());
            }
        }

        #endregion

        #region Depricated

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
                    shop.Product = connection.Query<Product>(subSql, new { shopId = shop.id }).ToList();
                    foreach (var product in shop.Product)
                        product.Image = connection.Query<Images>(sub2Sql, new { prodId = product.id }).FirstOrDefault();
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

                shop.Product = connection.Query<Product>(subSql, new { shopId = shop.id }).ToList();
                foreach (var product in shop.Product)
                    product.Image = connection.Query<Images>(sub2Sql, new { prodId = product.id }).FirstOrDefault();


                return shop;
            }
        }

        #endregion
    }
}
