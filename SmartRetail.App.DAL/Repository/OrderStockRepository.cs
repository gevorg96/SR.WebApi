using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Repository.Interfaces;
using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using SmartRetail.App.DAL.Helpers;

namespace SmartRetail.App.DAL.Repository
{
    public class OrderStockRepository : IOrderStockRepository
    {
        private readonly string conn;
        public OrderStockRepository(string connection)
        {
            conn = connection;
        }
        public async Task<IEnumerable<OrderStock>> GetOrderStocksByProdId(int prodId)
        {
            var sql = "select * from OrderStock where prod_id = " + prodId;
            using (var db = new SqlConnection(conn))
            {
                db.Open();
                return await db.QueryAsync<OrderStock>(sql);
            }
        }

        public async Task<IEnumerable<OrderStock>> GetOrderStocksByProdIds(IEnumerable<int> prodIds)
        {
            var sql = "select * from OrderStock where prod_id in (" + QueryHelper.GetIds(prodIds) + ");";
            using (var db = new SqlConnection(conn))
            {
                db.Open();
                return await db.QueryAsync<OrderStock>(sql);
            }
        }
    }
}
