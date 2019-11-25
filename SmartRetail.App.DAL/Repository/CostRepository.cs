using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Repository.Interfaces;
using System.Data.SqlClient;
using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using Npgsql;
using SmartRetail.App.DAL.UnitOfWork;
using static SmartRetail.App.DAL.Helpers.NullChecker;

namespace SmartRetail.App.DAL.Repository
{
    public class CostRepository : ICostRepository
    {
        private readonly string conn;
        private readonly IUnitOfWork _unitOfWork;


        public CostRepository(string conn)
        {
            this.conn = conn;
        }

        public CostRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> UpdateUow(Cost cost)
        {
            return await _unitOfWork.Connection.UpdateAsync(cost, transaction: _unitOfWork.Transaction);
        }

        public async Task<int> InsertUow(Cost cost)
        {
            return await _unitOfWork.Connection.InsertAsync(cost, transaction:_unitOfWork.Transaction);
        }

        public async Task<IEnumerable<Cost>> GetByProdIdUow(int prodId)
        {
            var sql = "select * from \"Costs\" where prod_id = " + prodId;
            return await _unitOfWork.Connection.QueryAsync<Cost>(sql, transaction:_unitOfWork.Transaction);
            
        }


        public IEnumerable<Cost> GetByProdId(int prodId)
        {
            var sql = "select * from \"Costs\" where prod_id = " + prodId;
            using (var db = new NpgsqlConnection(conn))
            {
                db.Open();
                return db.Query<Cost>(sql);
            }
        }

        public Cost GetByProdAndShopIds(int prodId, int shopId)
        {
            var sql = "select * from \"Costs\" where prod_id = " + prodId;
            using (var db = new NpgsqlConnection(conn))
            {
                db.Open();
                return db.QueryFirstOrDefault<Cost>(sql);
            }
        }

        public async Task UpdateCostValueAsync(Cost entity)
        {
            var sql = "update \"Costs\" set value = " + isNotNull(entity.value) + " where id = " + entity.id;
            using (var db = new NpgsqlConnection(conn))
            {
                db.Open();
                await db.ExecuteAsync(sql);
            }
        }

        public async Task AddCostAsync(Cost entity)
        {
            var sql = "insert into \"Costs\" (prod_id, value) values (@prodId, @value)";
            using (var db = new NpgsqlConnection(conn))
            {
                db.Open();
                await db.ExecuteAsync(sql, new { prodId = entity.prod_id, value = entity.value});
            }
        }
    }
}
