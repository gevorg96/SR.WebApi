using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using System.Threading.Tasks;
using System.Linq;
using static SmartRetail.App.DAL.Helpers.NullChecker;

namespace SmartRetail.App.DAL.Repository
{
    public class OrdersRepository : IOrdersRepository
    {
        private readonly string conn;
        public OrdersRepository(string connection)
        {
            conn = connection;
        }

        public async Task<Orders> GetByIdAsync(int orderId)
        {
            var sql = "select * from Orders where id = " + orderId;
            using (var db = new SqlConnection(conn))
            {
                db.Open();
                return await db.QueryFirstOrDefaultAsync<Orders>(sql);
            }
        }

        public async Task<Orders> GetByIdWithMultiAsync(int orderId)
        {
            var sql = "select * from Orders as o join OrderDetails as od on o.id = od.order_id where o.id = " + orderId;

            using (var db = new SqlConnection(conn))
            {
                db.Open();

                var orderDictionary = new Dictionary<int, Orders>();

                var orderStocks = await db.QueryAsync<Orders, OrderDetails, Orders>(sql,
                    (order, orderDetail) =>
                    {
                        Orders orderEntry;
                        if (!orderDictionary.TryGetValue(order.id, out orderEntry))
                        {
                            orderEntry = order;
                            orderEntry.OrderDetails = new List<OrderDetails>();
                            orderDictionary.Add(orderEntry.id, orderEntry);
                        }

                        orderEntry.OrderDetails.Add(orderDetail);
                        return orderEntry;

                    }, splitOn: "id");

                return orderStocks.Distinct().FirstOrDefault();
            }
        }

        public async Task<int> AddOrderAsync(Orders order)
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

        public async Task<int> AddCancellationAsync(Orders order)
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

        public async Task<IEnumerable<Orders>> GetOrdersByShopIdInDateRange(int shopId, DateTime from, DateTime to)
        {
            var sql = "select * from Orders as o join OrderDetails as od on o.id = od.order_id where o.shop_id = " + 
                shopId + " and o.report_date between '" + from.ToString("MM.dd.yyyy HH:mm:ss") + "' and '" 
                + to.ToString("MM.dd.yyyy HH:mm:ss") + "' and o.isOrder = 1";

            using (var db = new SqlConnection(conn))
            {
                db.Open();

                var orderDictionary = new Dictionary<int, Orders>();

                var orderStocks = await db.QueryAsync<Orders, OrderDetails, Orders>(sql,
                    (order, orderDetail) =>
                    {
                        Orders orderEntry;
                        if (!orderDictionary.TryGetValue(order.id, out orderEntry))
                        {
                            orderEntry = order;
                            orderEntry.OrderDetails = new List<OrderDetails>();
                            orderDictionary.Add(orderEntry.id, orderEntry);
                        }

                        orderEntry.OrderDetails.Add(orderDetail);
                        return orderEntry;

                    }, splitOn: "id");

                return orderStocks.Distinct().ToList();
            }
        }

        public async Task<IEnumerable<Orders>> GetCancellationsByShopIdInDateRange(int shopId, DateTime from, DateTime to)
        {
            var sql = "select * from Orders as o join OrderDetails as od on o.id = od.order_id where o.shop_id = " +
                shopId + " and o.report_date between '" + from.ToString("MM.dd.yyyy HH:mm:ss") + "' and '"
                + to.ToString("MM.dd.yyyy HH:mm:ss") + "' and o.isOrder = 0";

            using (var db = new SqlConnection(conn))
            {
                db.Open();

                var orderDictionary = new Dictionary<int, Orders>();

                var orderStocks = await db.QueryAsync<Orders, OrderDetails, Orders>(sql,
                    (order, orderDetail) =>
                    {
                        Orders orderEntry;
                        if (!orderDictionary.TryGetValue(order.id, out orderEntry))
                        {
                            orderEntry = order;
                            orderEntry.OrderDetails = new List<OrderDetails>();
                            orderDictionary.Add(orderEntry.id, orderEntry);
                        }

                        orderEntry.OrderDetails.Add(orderDetail);
                        return orderEntry;

                    }, splitOn: "id");

                return orderStocks.Distinct().ToList();
            }

        }

        //public async Task<Orders> GetLastOrderAsync(int shopId, DateTime from, DateTime to)
        //{
        //    var sql = "select * from Orders where shop_id = " + shopId + " and report_date between '" +
        //       from.ToString("MM.dd.yyyy HH:mm:ss") + "' and '" + to.ToString("MM.dd.yyyy HH:mm:ss") + "' and count > 0";
        //    using (var db = new SqlConnection(conn))
        //    {
        //        db.Open();
        //        var res = await db.QueryAsync<Orders>(sql);
        //        return res.AsList().Last();
        //    }
        //}

        public async Task<Orders> GetByShopIdOnDate(int shopId, DateTime reportDate, bool isOrder)
        {
            var sql = "select * from Orders where shop_id = " + shopId + " and report_date = '" +
               reportDate.ToString("MM.dd.yyyy HH:mm:ss") + "' and isOrder = " + (isOrder ? "1" : "0");
            using (var db = new SqlConnection(conn))
            {
                db.Open();
                var result = await db.QueryAsync<Orders>(sql);
                return result.Last();
            }
        }
    }
}
