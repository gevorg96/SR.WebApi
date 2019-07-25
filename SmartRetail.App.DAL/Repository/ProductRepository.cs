using System;
using System.Data;
using Dapper;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Helpers;
using static SmartRetail.App.DAL.Helpers.NullChecker;
using System.Threading.Tasks;

namespace SmartRetail.App.DAL.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly string _connectionString;
        private QueryBuilder qb;

        public ProductRepository(string conn)
        {
            _connectionString = conn;
            qb = new QueryBuilder();
        }

        #region Read

        public IEnumerable<Product> GetWithFilter(string field, string value)
        {
            var sql = "SELECT * FROM Product WHERE " + field + " = " + value;
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open();
                return db.Query<Product>(sql).ToList();
            }
        }

        public IEnumerable<Product> GetAll()
        {
            var sql = "SELECT * FROM Product";
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open();
                return db.Query<Product>(sql).ToList();
            }
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            var sql = "SELECT * FROM Product WHERE id = " + id;
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open();
                return await db.QueryFirstOrDefaultAsync<Product>(sql);
            }
        }

        public async Task<Product> GetByIdAsync(int id, int businessId)
        {
            var sql = "select * from Product where business_id = @BusinessId and id = @Id";
            using (var db = new SqlConnection(_connectionString))
            {
                db.Open();
                return await db.QueryFirstOrDefaultAsync<Product>(sql, new { BusinessId = businessId, Id = id });
            }
        }

        public IEnumerable<Product> GetProductsByIds(IEnumerable<int> prodIds)
        {
            qb.Clear();
            var sql = "select * from Product where id in (" + QueryHelper.GetIds(prodIds) + ")";

            using (var db = new SqlConnection())
            {
                db.Open();
                return db.Query<Product>(sql);
            }
        }

        public async Task<IEnumerable<Product>> GetProductsByBusinessAsync(int businessId)
        {
            var prsql = "select * from Product where business_id = " + businessId;
            var imgSql = "select * from Images where prod_id = @prodId";

            IEnumerable<Product> prods = new List<Product>();
            using (var db = new SqlConnection(_connectionString))
            {
                db.Open();
                prods = await db.QueryAsync<Product>(prsql);
                foreach (var prod in prods)
                {
                    prod.Image = await db.QueryFirstOrDefaultAsync<Images>(imgSql, new { prodId = prod.id });
                }
                return prods;
            }
        }

        #endregion

        #region Create

        public int AddProduct(Product entity)
        {
            //shop_id,
            string prodSql = string.Format("INSERT INTO Product ( business_id, supplier_id, name, attr1, " +
                                           "attr2, attr3, attr4, attr5, attr6, attr7, attr8, attr9, attr10, unit_id) Values ( {0}, {1}, {2}, " +
                                           "{3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13});", isNotNull(entity.business_id),
                isNotNull(entity.supplier_id), isNotNull(entity.name), isNotNull(entity.attr1), isNotNull(entity.attr2),
                isNotNull(entity.attr3), isNotNull(entity.attr4), isNotNull(entity.attr5), isNotNull(entity.attr6),
                isNotNull(entity.attr7), isNotNull(entity.attr8), isNotNull(entity.attr9), isNotNull(entity.attr10), isNotNull(entity.unit_id));

            string priceSql = "INSERT INTO Price (prod_id, price, shop_id) VALUES (@prodId, @Price, @shopId)";

            var selectSql = "SELECT * FROM Product WHERE business_id = " + entity.business_id + " AND name = N'" + entity.name + "'";

            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Execute(prodSql);
                    var rows = connection.Query<Product>(selectSql);
                    var id = GetMaxId(rows);

                    if (id != 0)
                    {
                        var price = entity.Prices.FirstOrDefault();
                        if (price != null && price?.price != null)
                        {
                            connection.Execute(priceSql,
                                new { prodId = id, Price = price.price, shopId = entity.shop_id });
                        }
                        return id;
                    }

                    return 0;
                }
                catch (Exception e)
                {
                    throw new Exception("Что-то не так: " + e.Message);
                }
            }
        }

        #endregion

        #region Update

        public void UpdateProduct(Product entity, string field)
        {
            var pi = entity.GetType().GetProperty(field);
            var value = pi.GetValue(entity);
            var sql = "update Product set " + field + " = " + QueryHelper.GetSqlString(pi,value);
            using (var db = new SqlConnection(_connectionString))
            {
                try
                {
                    db.Execute(sql);
                }
                catch (Exception e)
                {
                    throw new Exception("Что-то пошло не так: " + e.Message);
                }
                
            }
        }

        public void UpdateProduct(Product entity)
        {
            qb.Clear();
            var select = qb.Select("*").From("Product").Where("id").Op(Ops.Equals, entity.id.ToString());
            var sb = new StringBuilder();
            sb.Append("update Product set ");

            var pi = entity.GetType().GetProperties();
            
            using(var db = new SqlConnection(_connectionString))
            {
                db.Open();
                var row = db.QueryFirstOrDefault<Product>(select.ToString());
                for (var i = 1; i < 16; i++)
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
                db.Execute(sb.ToString());
            }
        }

        #endregion

        #region Helpers

        private int GetMaxId(IEnumerable<Product> products, int? shopId = null)
        {
            IEnumerable<int> ids;
            if (shopId.HasValue)
            {
                products = products.Where(p => p.shop_id == shopId);
            }

            ids = products.Select(p => p.id);
            return ids.Max();
        }


        #endregion

        #region Depricated

        public IEnumerable<Product> GetProducts(int id)
        {
            var sql = "SELECT * FROM Product WHERE id > " + id;
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open();
                return db.Query<Product>(sql);
            }
        }

        #endregion

    }
}
