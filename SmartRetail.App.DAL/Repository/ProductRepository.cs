﻿using System;
using System.Data;
using Dapper;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Helpers;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using Npgsql;
using SmartRetail.App.DAL.Repository.Interfaces;
using SmartRetail.App.DAL.UnitOfWork;
using static SmartRetail.App.DAL.Helpers.NullChecker;

namespace SmartRetail.App.DAL.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly string _connectionString;
        private QueryBuilder qb;
        private IUnitOfWork _unitOfWork;

        public ProductRepository(IUnitOfWork unitOfWork)
        {
            qb = new QueryBuilder();
            _unitOfWork = unitOfWork;
        }

        public ProductRepository(string conn)
        {
            _connectionString = conn;
            qb = new QueryBuilder();
        }


        public async Task<int> InsertUow(Product product)
        {
            return await _unitOfWork.Connection.InsertAsync(product, _unitOfWork.Transaction);
        }

        public async Task<bool> UpdateUow(Product product)
        {
            return await _unitOfWork.Connection.UpdateAsync(product, _unitOfWork.Transaction);
        }

        public async Task<Product> GetByIdUow(int id)
        {
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<Product>(
                "select * from \"Products\" where id = " + id, null, _unitOfWork.Transaction);
        }

        #region WithOut Transactions

        #region Read

        public async Task<Product> GetByIdAsync(int id)
        {
            var sql = "SELECT * FROM \"Products\" WHERE id = " + id;
            using (IDbConnection db = new NpgsqlConnection(_connectionString))
            {
                db.Open();
                return await db.QueryFirstOrDefaultAsync<Product>(sql);
            }
        }

        public async Task<Product> GetByIdAsync(int id, int businessId)
        {
            var sql = "select * from \"Products\" where business_id = @BusinessId and id = @Id";
            using (var db = new NpgsqlConnection(_connectionString))
            {
                db.Open();
                return await db.QueryFirstOrDefaultAsync<Product>(sql, new { BusinessId = businessId, Id = id });
            }
        }

        public IEnumerable<Product> GetProductsByIds(IEnumerable<int> prodIds)
        {
            qb.Clear();
            var sql = "select * from \"Products\" where id in (" + QueryHelper.GetIds(prodIds) + ")";

            using (var db = new NpgsqlConnection())
            {
                db.Open();
                return db.Query<Product>(sql);
            }
        }

        public async Task<IEnumerable<Product>> GetProductsByBusinessAsync(int businessId)
        {
            var prsql = "select * from \"Products\" where business_id = " + businessId;
            var imgSql = "select * from \"Images\" where prod_id = @prodId";

            IEnumerable<Product> prods = new List<Product>();
            using (var db = new NpgsqlConnection(_connectionString))
            {
                db.Open();
                prods = await db.QueryAsync<Product>(prsql);
                foreach (var prod in prods)
                {
                    prod.Image = await db.QueryFirstOrDefaultAsync<Image>(imgSql, new { prodId = prod.id });
                }
                return prods;
            }
        }

        #endregion

        #region Create

        public int AddProduct(Product entity)
        {
            //shop_id,
            string prodSql = string.Format("INSERT INTO \"Products\" ( business_id, supplier_id, name, attr1, " +
                                           "attr2, attr3, attr4, attr5, attr6, attr7, attr8, attr9, attr10, unit_id, folder_id) Values ( {0}, {1}, {2}, " +
                                           "{3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14});", isNotNull(entity.business_id),
                isNotNull(entity.supplier_id), isNotNull(entity.name), isNotNull(entity.attr1), isNotNull(entity.attr2),
                isNotNull(entity.attr3), isNotNull(entity.attr4), isNotNull(entity.attr5), isNotNull(entity.attr6),
                isNotNull(entity.attr7), isNotNull(entity.attr8), isNotNull(entity.attr9), isNotNull(entity.attr10), isNotNull(entity.unit_id),
                isNotNull(entity.folder_id));

            string priceSql = "INSERT INTO \"Prices\" (prod_id, price) VALUES (@prodId, @Price)";

            var selectSql = "SELECT * FROM \"Products\" WHERE business_id = " + entity.business_id + " AND name = N'" + entity.name + "'";

            using (var connection = new NpgsqlConnection(_connectionString))
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
                                new { prodId = id, Price = isNotNull(price.price) });
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

        public void UpdateProduct(Product entity)
        {
            qb.Clear();
            var select = qb.Select("*").From("\"Products\"").Where("id").Op(Ops.Equals, entity.id.ToString());
            var sb = new StringBuilder();
            sb.Append("update \"Products\" set ");

            var pi = entity.GetType().GetProperties();

            using (var db = new NpgsqlConnection(_connectionString))
            {
                db.Open();
                var row = db.QueryFirstOrDefault<Product>(select.ToString());
                for (var i = 1; i < 17; i++)
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

        private int GetMaxId(IEnumerable<Product> products)
        {
            return products.Select(p => p.id).Max();
        }

        #endregion

        #endregion


    }
}
