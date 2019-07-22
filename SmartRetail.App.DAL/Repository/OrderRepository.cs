using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using System.Threading.Tasks;
using SmartRetail.App.DAL.Helpers;

namespace SmartRetail.App.DAL.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly string conn;

        public OrderRepository(string connection)
        {
            conn = connection;
        }

        public async Task AddOrderAsync(Orders entity)
        {
            var sql = "insert into Orders (prod_id, cost, count, report_date) values(" + 
                entity.prod_id +", " + entity.cost + ", " + entity.count + ", '" + entity.report_date.Value.ToString("MM.dd.yyyy HH:mm:ss") + "')";
            using (var db = new SqlConnection(conn))
            {
                db.Open();
                await db.ExecuteAsync(sql);
            }
        }

        public async Task<IEnumerable<Orders>> GetOrdersByProdId(int prodId)
        {
            var sql = "select * from Orders where prod_id = " + prodId;
            using (var db = new SqlConnection(conn))
            {
                db.Open();
                return await db.QueryAsync<Orders>(sql); 
            }
        }

        public async Task<IEnumerable<Orders>> GetOrdersByProdIds(IEnumerable<int> prodIds)
        {
            var sql = "select * from Orders where prod_id in (" + QueryHelper.GetIds(prodIds) +");";
            using (var db = new SqlConnection(conn))
            {
                db.Open();
                return await db.QueryAsync<Orders>(sql);
            }
        }
    }
}
