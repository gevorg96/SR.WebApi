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

        //public Task Add(Orders entity)
        //{
        //    var sql = "insert into Orders (prod_id, cost_id, count, stock_sale, report_date) values( {0}, {1}, {2}, {3}, {4})";
        //    return new Task(()=> 2*2);
        //}

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
