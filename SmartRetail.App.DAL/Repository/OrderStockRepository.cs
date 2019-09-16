using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Repository.Interfaces;
using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using SmartRetail.App.DAL.UnitOfWork;
using static SmartRetail.App.DAL.Helpers.NullChecker;

namespace SmartRetail.App.DAL.Repository
{
    public class OrderStockRepository : IOrderStockRepository
    {
        private readonly string conn;
        private readonly IUnitOfWork _unitOfWork;

        public OrderStockRepository(string connection)
        {
            conn = connection;
        }

        public OrderStockRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> InsertUow(OrderStock orderStock)
        {
            return await _unitOfWork.Connection.InsertAsync(orderStock, transaction: _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<OrderStock>> GetPureOrderStocksByProdAndShopIdsUow(int prodId, int shopId)
        {
            var sql = "select * from OrderStocks as OS join OrderDetails as O ON OS.order_id = O.id where OS.prod_id = "
                      + prodId + " and OS.curr_stocks != 0 and OS.shop_id = " + shopId + " order by OS.order_id";

            var orderStocks = await _unitOfWork.Connection.QueryAsync<OrderStock, OrderDetail, OrderStock>(sql,
                    (orderStock, orderDetail) =>
                    {
                        orderStock.OrderDetail = orderDetail;
                        return orderStock;
                    }, splitOn: "id", transaction: _unitOfWork.Transaction);
                return orderStocks.AsList();
            
        }



        public async Task<IEnumerable<OrderStock>> GetPureOrderStocksByProdIdUow(int prodId)
        {
            var sql = "select * from OrderStocks as OS join OrderDetails as O ON OS.order_id = O.id where OS.prod_id = "
                      + prodId + " and OS.curr_stocks != 0 order by OS.order_id";

            var orderStocks = await _unitOfWork.Connection.QueryAsync<OrderStock, OrderDetail, OrderStock>(sql,
                    (orderStock, orderDetail) =>
                    {
                        orderStock.OrderDetail = orderDetail;
                        return orderStock;
                    }, splitOn: "id", transaction: _unitOfWork.Transaction);
                return orderStocks.AsList();
            
        }

        #region Without Transactions

        public async Task AddOrderStockAsync(OrderStock entity)
        {
            var sql = "insert into OrderStocks (order_id, prod_id, curr_stocks, shop_id) values (" + entity.order_id + ", " + entity.prod_id + ", " + entity.curr_stocks + ", " + entity.shop_id + ")";
            using (var db = new SqlConnection(conn))
            {
                db.Open();
                await db.ExecuteAsync(sql);
            }
        }

        public async Task<IEnumerable<OrderStock>> GetPureOrderStocksByProdAndShopIds(int prodId, int shopId)
        {
            var sql = "select * from OrderStocks as OS join OrderDetails as O ON OS.order_id = O.id where OS.prod_id = "
                + prodId + " and OS.curr_stocks != 0 and OS.shop_id = " + shopId + " order by OS.order_id";

            using (var db = new SqlConnection(conn))
            {
                db.Open();
                var orderStocks = await db.QueryAsync<OrderStock, OrderDetail, OrderStock>(sql,
                    (orderStock, orderDetail) =>
                    {
                        orderStock.OrderDetail = orderDetail;
                        return orderStock;
                    }, splitOn: "id", transaction:_unitOfWork.Transaction);
                return orderStocks.AsList();
            }
        }

        public async Task<IEnumerable<OrderStock>> GetPureOrderStocksByProdId(int prodId)
        {
            var sql = "select * from OrderStocks as OS join OrderDetails as O ON OS.order_id = O.id where OS.prod_id = "
                + prodId + " and OS.curr_stocks != 0 order by OS.order_id";

            using (var db = new SqlConnection(conn))
            {
                db.Open();
                var orderStocks = await db.QueryAsync<OrderStock, OrderDetail, OrderStock>(sql,
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
            var sql = "update OrderStocks set curr_stocks = " + isNotNull(entity.curr_stocks) + " where id = " + entity.id;
            using (var db = new SqlConnection(conn))
            {
                db.Open();
                await db.ExecuteAsync(sql);
            }
        }

        #endregion
    }
}
