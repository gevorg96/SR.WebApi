using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using System.Threading.Tasks;
using System.Linq;
using Dapper.Contrib.Extensions;
using SmartRetail.App.DAL.UnitOfWork;

namespace SmartRetail.App.DAL.Repository
{
    public class OrdersRepository : IOrdersRepository
    {
        private readonly string conn;
        private IUnitOfWork _unitOfWork;


        public OrdersRepository(string connection)
        {
            conn = connection;
        }

        public OrdersRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<int> InsertUow(Order order)
        {
            return await _unitOfWork.Connection.InsertAsync(order, transaction:_unitOfWork.Transaction);
        }

        public async Task<Order> GetByIdWithMultiUow(int orderId)
        {
            var sql = "select * from Orders as o join OrderDetails as od on o.id = od.order_id where o.id = " + orderId;

            var orderDictionary = new Dictionary<int, Order>();

            var orderStocks = await _unitOfWork.Connection.QueryAsync<Order, OrderDetail, Order>(sql,
                (order, orderDetail) =>
                {
                    Order orderEntry;
                    if (!orderDictionary.TryGetValue(order.id, out orderEntry))
                    {
                        orderEntry = order;
                        orderEntry.OrderDetails = new List<OrderDetail>();
                        orderDictionary.Add(orderEntry.id, orderEntry);
                    }

                    orderEntry.OrderDetails.Add(orderDetail);
                    return orderEntry;

                }, splitOn: "id", transaction: _unitOfWork.Transaction);

            return orderStocks.Distinct().FirstOrDefault();
        }

        #region Without Transactions

        public async Task<Order> GetByIdAsync(int orderId)
        {
            var sql = "select * from Orders where id = " + orderId;
            var detailSql = "select * from OrderDetails where order_id = " + orderId;
            using (var db = new SqlConnection(conn))
            {
                db.Open();
                var order =  await db.QueryFirstOrDefaultAsync<Order>(sql);
                order.OrderDetails = (await db.QueryAsync<OrderDetail>(detailSql)).ToList();
                return order;
            }
        }

        public async Task<Order> GetByIdWithMultiAsync(int orderId)
        {
            var sql = "select * from Orders as o join OrderDetails as od on o.id = od.order_id where o.id = " + orderId;

            using (var db = new SqlConnection(conn))
            {
                db.Open();

                var orderDictionary = new Dictionary<int, Order>();

                var orderStocks = await db.QueryAsync<Order, OrderDetail, Order>(sql,
                    (order, orderDetail) =>
                    {
                        Order orderEntry;
                        if (!orderDictionary.TryGetValue(order.id, out orderEntry))
                        {
                            orderEntry = order;
                            orderEntry.OrderDetails = new List<OrderDetail>();
                            orderDictionary.Add(orderEntry.id, orderEntry);
                        }

                        orderEntry.OrderDetails.Add(orderDetail);
                        return orderEntry;

                    }, splitOn: "id");

                return orderStocks.Distinct().FirstOrDefault();
            }
        }

        public async Task<int> AddOrderAsync(Order order)
        {
            if (order.OrderDetails == null || !order.OrderDetails.Any())
            {
                return 0;
            }
            var sql = "insert into Orders (report_date, shop_id, isOrder) values ('" + order.report_date.ToString("MM.dd.yyyy HH:mm:ss") +"', " + order.shop_id + ", 1)";
            var odSql = "insert into OrderDetails (order_id, prod_id, cost, count) values (@OrderId, @ProdId, @Cost, @Count)";

            using (var db = new SqlConnection(conn))
            {
                db.Open();
                await db.ExecuteAsync(sql);
                var orderDal = await GetByShopIdOnDate(order.shop_id, order.report_date, true);
                foreach (var item in order.OrderDetails)
                {
                    await db.ExecuteAsync(odSql, new { OrderId = orderDal.id, ProdId = item.prod_id, Cost = item.cost, Count = item.count });
                }
                return orderDal.id;
            }
        }

        public async Task<int> AddCancellationAsync(Order order)
        {
            if (order.OrderDetails == null || !order.OrderDetails.Any())
            {
                return 0;
            }
            var sql = "insert into Orders (report_date, shop_id, isOrder) values ('" + order.report_date.ToString("MM.dd.yyyy HH:mm:ss") + "', " + order.shop_id + ", 0)";
            var odSql = "insert into OrderDetails (order_id, prod_id, cost, count) values (@OrderId, @ProdId, @Cost, @Count)";

            using (var db = new SqlConnection(conn))
            {
                db.Open();
                await db.ExecuteAsync(sql);
                var orderDal = await GetByShopIdOnDate(order.shop_id, order.report_date, false);
                foreach (var item in order.OrderDetails)
                {
                    await db.ExecuteAsync(odSql, new { OrderId = orderDal.id, ProdId = item.prod_id, Cost = item.cost, Count = item.count });
                }
                return orderDal.id;
            }
        }

        public async Task<IEnumerable<Order>> GetOrdersByShopIdInDateRange(int shopId, DateTime from, DateTime to)
        {
            var sql = "select * from Orders as o join OrderDetails as od on o.id = od.order_id where o.shop_id = " + 
                shopId + " and o.report_date between '" + from.ToString("MM.dd.yyyy HH:mm:ss") + "' and '" 
                + to.ToString("MM.dd.yyyy HH:mm:ss") + "' and o.isOrder = 1";

            using (var db = new SqlConnection(conn))
            {
                db.Open();

                var orderDictionary = new Dictionary<int, Order>();

                var orderStocks = await db.QueryAsync<Order, OrderDetail, Order>(sql,
                    (order, orderDetail) =>
                    {
                        Order orderEntry;
                        if (!orderDictionary.TryGetValue(order.id, out orderEntry))
                        {
                            orderEntry = order;
                            orderEntry.OrderDetails = new List<OrderDetail>();
                            orderDictionary.Add(orderEntry.id, orderEntry);
                        }

                        orderEntry.OrderDetails.Add(orderDetail);
                        return orderEntry;

                    }, splitOn: "id");

                return orderStocks.Distinct().ToList();
            }
        }

        public async Task<IEnumerable<Order>> GetCancellationsByShopIdInDateRange(int shopId, DateTime from, DateTime to)
        {
            var sql = "select * from Orders as o join OrderDetails as od on o.id = od.order_id where o.shop_id = " +
                shopId + " and o.report_date between '" + from.ToString("MM.dd.yyyy HH:mm:ss") + "' and '"
                + to.ToString("MM.dd.yyyy HH:mm:ss") + "' and o.isOrder = 0";

            using (var db = new SqlConnection(conn))
            {
                db.Open();

                var orderDictionary = new Dictionary<int, Order>();

                var orderStocks = await db.QueryAsync<Order, OrderDetail, Order>(sql,
                    (order, orderDetail) =>
                    {
                        Order orderEntry;
                        if (!orderDictionary.TryGetValue(order.id, out orderEntry))
                        {
                            orderEntry = order;
                            orderEntry.OrderDetails = new List<OrderDetail>();
                            orderDictionary.Add(orderEntry.id, orderEntry);
                        }

                        orderEntry.OrderDetails.Add(orderDetail);
                        return orderEntry;

                    }, splitOn: "id");

                return orderStocks.Distinct().ToList();
            }

        }

        public async Task<Order> GetByShopIdOnDate(int shopId, DateTime reportDate, bool isOrder)
        {
            var sql = "select * from Orders where shop_id = " + shopId + " and report_date = '" +
               reportDate.ToString("MM.dd.yyyy HH:mm:ss") + "' and isOrder = " + (isOrder ? "1" : "0");
            using (var db = new SqlConnection(conn))
            {
                db.Open();
                var result = await db.QueryAsync<Order>(sql);
                return result.Last();
            }
        }

        #endregion
    }
}
