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
            var sql = "insert into Orders (prod_id, cost, count, report_date, shop_id) values(" + 
                entity.prod_id +", " + entity.cost + ", " + entity.count + ", '" + 
                entity.report_date.Value.ToString("MM.dd.yyyy HH:mm:ss") + "', " +entity.shop_id + ");" ;
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

        public async Task<IEnumerable<Orders>> GetOrdersByShopId(int shopId, DateTime from, DateTime to)
        {
            var sql = "select * from Orders where shop_id = " + shopId + " and report_date between '" + 
                from.ToString("MM.dd.yyyy HH:mm:ss") + "' and '" + to.ToString("MM.dd.yyyy HH:mm:ss") + "' and count > 0";
            using (var db = new SqlConnection(conn))
            {
                return await db.QueryAsync<Orders>(sql);
            }
        }

        public async Task<Orders> GetLastOrderAsync(int shopId, int prodId, DateTime from, DateTime to)
        {
            var sql = "select * from Orders where shop_id = " + shopId + " and report_date between '" +
               from.ToString("MM.dd.yyyy HH:mm:ss") + "' and '" + to.ToString("MM.dd.yyyy HH:mm:ss") + "' and count > 0";
            using (var db = new SqlConnection(conn))
            {
                var res = await db.QueryAsync<Orders>(sql);
                return res.AsList().FindLast(p => p.prod_id == prodId);
            }
        }

        public async Task<IEnumerable<Orders>> GetCancellationsByShopId(int shopId, DateTime from, DateTime to)
        {
            var sql = "select * from Orders where shop_id = " + shopId + " and report_date between '" +
                from.ToString("MM.dd.yyyy HH:mm:ss") + "' and '" + to.ToString("MM.dd.yyyy HH:mm:ss") + "' and count < 0";
            using (var db = new SqlConnection(conn))
            {
                return await db.QueryAsync<Orders>(sql);
            }
        }

        public async Task<Orders> GetLastCancellationAsync(int shopId, int prodId, DateTime from, DateTime to)
        {
            var sql = "select * from Orders where shop_id = " + shopId + " and report_date between '" +
               from.ToString("MM.dd.yyyy HH:mm:ss") + "' and '" + to.ToString("MM.dd.yyyy HH:mm:ss") + "' and count < 0";
            using (var db = new SqlConnection(conn))
            {
                var res = await db.QueryAsync<Orders>(sql);
                return res.AsList().FindLast(p => p.prod_id == prodId);
            }
        }
    }
}
