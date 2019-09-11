using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Repository.Interfaces;
using static SmartRetail.App.DAL.Helpers.NullChecker;

namespace SmartRetail.App.DAL.Repository
{
    public class PriceRepository : IPriceRepository
    {
        private readonly string _connectionString;

        public PriceRepository(string conn)
        {
            _connectionString = conn;
        }

        public void Add(Price entity)
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

        public Price GetPriceByProdId(int prodId)
        {
            var sql = "select * from Price where prod_id = " + prodId;
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open();
                return db.Query<Price>(sql).FirstOrDefault();
            }
        }

        public void Update(Price entity)
        {
            var update = "update Price set price = " + isNotNull(entity.price) +
                ", shop_id = " + isNotNull(entity.shop_id) + " where prod_id = " + entity.prod_id;

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
    }
}