using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Repository.Interfaces;
using SmartRetail.App.DAL.UnitOfWork;
using static SmartRetail.App.DAL.Helpers.NullChecker;

namespace SmartRetail.App.DAL.Repository
{
    public class PriceRepository : IPriceRepository
    {
        private readonly string _connectionString;
        private readonly IUnitOfWork _unitOfWork;
        public PriceRepository(string conn)
        {
            _connectionString = conn;
        }

        public PriceRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }

        public async Task<int> InsertUow(Price price)
        {
            return await _unitOfWork.Connection.InsertAsync(price, _unitOfWork.Transaction);
        }

        public async Task<bool> UpdateUow(Price price)
        {
            return await _unitOfWork.Connection.UpdateAsync(price, transaction: _unitOfWork.Transaction);
        }

        #region Without Transactions

        public void Add(Price entity)
        {
            var insert = "insert into Prices (prod_id, price, shop_id) values (" + entity.prod_id + ", " + isNotNull(entity.price) + ", " +
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
            var sql = "select * from Prices where prod_id = " + prodId;
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open();
                return db.Query<Price>(sql).FirstOrDefault();
            }
        }

        public void Update(Price entity)
        {
            var update = "update Prices set price = " + isNotNull(entity.price) +
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

        #endregion
    }
}