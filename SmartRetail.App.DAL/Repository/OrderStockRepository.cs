using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Repository.Interfaces;
using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using SmartRetail.App.DAL.Helpers;
using static SmartRetail.App.DAL.Helpers.NullChecker;

namespace SmartRetail.App.DAL.Repository
{
    public class OrderStockRepository : IOrderStockRepository
    {
        private readonly string conn;
        public OrderStockRepository(string connection)
        {
            conn = connection;
        }

        public async Task AddOrderStockAsync(OrderStock entity)
        {
            var sql = "insert into OrderStock (order_id, prod_id, curr_stocks, shop_id) values (" + entity.order_id + ", " + entity.prod_id + ", " + entity.curr_stocks + ", " + entity.shop_id + ")";
            using (var db = new SqlConnection(conn))
            {
                db.Open();
                await db.ExecuteAsync(sql);
            }
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

        public async Task<IEnumerable<OrderStock>> GetPureOrderStocksByProdAndShopIds(int prodId, int shopId)
        {
            var sql = "select * from OrderStock as OS join OrderDetails as O ON OS.order_id = O.id where OS.prod_id = "
                + prodId + " and OS.curr_stocks != 0 and OS.shop_id = " + shopId + " order by OS.order_id";

            using (var db = new SqlConnection(conn))
            {
                db.Open();
                var orderStocks = await db.QueryAsync<OrderStock, OrderDetails, OrderStock>(sql,
                    (orderStock, orderDetail) =>
                    {
                        orderStock.OrderDetail = orderDetail;
                        return orderStock;
                    }, splitOn: "id");
                return orderStocks.AsList();
            }
        }

        public async Task<IEnumerable<OrderStock>> GetPureOrderStocksByProdId(int prodId)
        {
            var sql = "select * from OrderStock as OS join OrderDetails as O ON OS.order_id = O.id where OS.prod_id = "
                + prodId + " and OS.curr_stocks != 0 order by OS.order_id";

            using (var db = new SqlConnection(conn))
            {
                db.Open();
                var orderStocks = await db.QueryAsync<OrderStock, OrderDetails, OrderStock>(sql,
                    (orderStock, orderDetail) =>
                    {
                        orderStock.OrderDetail = orderDetail;
                        return orderStock;
                    }, splitOn: "id");
                return orderStocks.AsList();
            }
        }

        public async Task UpdateOrderStockAsync(OrderStock entity)
        {
            var sql = "update OrderStock set curr_stocks = " + isNotNull(entity.curr_stocks) + " where id = " + entity.id;
            using (var db = new SqlConnection(conn))
            {
                db.Open();
                await db.ExecuteAsync(sql);
            }
        }
    }
}
