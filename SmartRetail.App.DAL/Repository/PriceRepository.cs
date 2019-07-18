using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Helpers;
using static SmartRetail.App.DAL.Helpers.NullChecker;

namespace SmartRetail.App.DAL.Repository
{
    public class PriceRepository : EntityRepository<Price>, IPriceRepository
    {
        private readonly string _connectionString;

        public PriceRepository(string conn)
        {
            _connectionString = conn;
        }

        public new void Add(Price entity)
        {
            var insert = "insert into Price (prod_id, price, shop_id) values (" + entity.prod_id + ", " + isNotNull(entity.price) + ", " +
                isNotNull(entity.shop_id) + ")";
            using (var db = new SqlConnection(_connectionString))
            {
                db.Open();
                try
                {
                    db.Execute(insert);
                }
                catch (Exception ex)
                {
                    throw new Exception("Что-то пошло не так: " + ex.Message);
                }
            }
        }

        public IEnumerable<Price> GetPricesByValue(string field, string value)
        {

            var sql = "SELECT * FROM Price WHERE " + field + " = " + value;
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open();
                return db.Query<Price>(sql).ToList();

            }

        }

        public IEnumerable<Price> GetPricesByIds(IEnumerable<int> ids)
        {

            var sql = "SELECT * FROM Price WHERE prod_id in ( " + QueryHelper.GetIds(ids) + " )";
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open();
                return db.Query<Price>(sql).ToList();

            }
        }

        public Price GetPriceByProdId(int prodId)
        {
            var sql = "select * from Price where prod_id = " + prodId;
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open();
                return db.Query<Price>(sql).FirstOrDefault();
            }
        }

        public new void Update(Price entity)
        {
            var update = "update Price set price = " + entity.price +
                ", shop_id = " + entity.shop_id + " where prod_id = " + entity.prod_id;

            using (var db = new SqlConnection(_connectionString))
            {
                db.Open();
                try
                {
                    db.Execute(update);
                }
                catch (Exception ex)
                {
                    throw new Exception("Что-то пошло не так: " + ex.Message);
                }
            }
        }

        public Price GetPriceByProdAndShopIds(int prodId, int shopId)
        {
            var sql = "select * from Price where prod_id = " + prodId + " and shop_id = " + shopId;
            using (var db = new SqlConnection(_connectionString))
            {
                db.Open();
                return db.Query<Price>(sql).FirstOrDefault();
            }
        }
    }
}