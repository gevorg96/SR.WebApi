﻿using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using System.Threading.Tasks;
using System.Linq;
using Dapper.Contrib.Extensions;
using Npgsql;
using SmartRetail.App.DAL.UnitOfWork;
using static SmartRetail.App.DAL.Helpers.NullChecker;

namespace SmartRetail.App.DAL.Repository
{
    public class BillsRepository : IBillsRepository
    {
        private readonly string conn;
        private IUnitOfWork _unitOfWork;
        public BillsRepository(string connection)
        {
            conn = connection;
        }

        public BillsRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> InsertUow(Bill entity)
        {
            if (entity.Sales == null || !entity.Sales.Any())
                return -1;

            entity.bill_number = await GetMaxBillNumberInDay(entity.shop_id, entity.report_date);

            var billId = await _unitOfWork.Connection.InsertAsync(entity, _unitOfWork.Transaction);
            foreach (var sale in entity.Sales)
            {
                sale.bill_id = billId;
                await _unitOfWork.Connection.InsertAsync(sale, _unitOfWork.Transaction);
            }

            return billId;
        }

        public async Task<IEnumerable<Bill>> GetBillsWithSalesByProdId(int shopId, int prodId, DateTime from,
            DateTime to)
        {
            var join = "select * from \"Bills\" as b join \"Sales\" as s on b.id = s.bill_id where b.shop_id = " + 
                       shopId + " and b.report_date between '" + from.ToString("MM.dd.yyyy HH:mm:ss") + "' and '" 
                       + to.ToString("MM.dd.yyyy HH:mm:ss") + "' and s.prod_id = " + prodId;
            var prodSql = "select * from \"Products\" where id = @ProdId";
            var priceSelect = "select * from \"Prices\" where prod_id = @ProdId";

            using (var db = new NpgsqlConnection(conn))
            {
                db.Open();
                var billDict = new Dictionary<int, Bill>();

                var bills = (await db.QueryAsync<Bill, Sale, Bill>(join,
                    (bill, sales) =>
                    {
                        Bill billEntry;
                        if (!billDict.TryGetValue(bill.id, out billEntry))
                        {
                            billEntry = bill;
                            billEntry.Sales = new List<Sale>();
                            billDict.Add(billEntry.id, billEntry);
                        }

                        billEntry.Sales.Add(sales);
                        return billEntry;
                    },
                    splitOn: "id")).Distinct().ToList();
                foreach (var bill in bills)
                {
                    foreach (var sale in bill.Sales)
                    {
                        sale.Product = await db.QueryFirstOrDefaultAsync<Product>(prodSql, new {ProdId = sale.prod_id});
                        if (sale.Product != null)
                            sale.Product.Price =
                                await db.QueryFirstOrDefaultAsync<Price>(priceSelect, new {ProdId = sale.prod_id});
                    }
                }

                return bills;
            }
        }
        
        public async Task<Bill> GetByIdsWithSalesUow(int billId, int shopId)
        {
            var sql = "select * from \"Bills\" as b join \"Sales\" as s on b.id = s.bill_id where b.id = "+ billId +" and b.shop_id = " + shopId;

            var biilsDictionary = new Dictionary<int, Bill>();

            var billsStocks = await _unitOfWork.Connection.QueryAsync<Bill, Sale, Bill>(sql,
                (bill, sale) =>
                {
                    Bill billEntry;
                    if (!biilsDictionary.TryGetValue(bill.id, out billEntry))
                    {
                        billEntry = bill;
                        billEntry.Sales = new List<Sale>();
                        biilsDictionary.Add(billEntry.id, billEntry);
                    }

                    billEntry.Sales.Add(sale);
                    return billEntry;

                }, splitOn: "id", transaction: _unitOfWork.Transaction);

            return billsStocks.Distinct().FirstOrDefault();
        
        }

        public async Task<int> AddBillAsync(Bill bill)
        {
            if (bill.Sales == null || !bill.Sales.Any())
                return -1;

            var nextBillNumber = await GetMaxBillNumberInDay(bill.shop_id, bill.report_date);

            var insert = "insert into \"Bills\" (shop_id, bill_number, report_date, sum) values(" + bill.shop_id +", "
                + nextBillNumber + ", '" + bill.report_date.ToString("MM.dd.yyyy HH:mm:ss") + "', " + bill.sum + ")";

            var sql = "INSERT INTO \"Sales\" (bill_id, prod_id, count, sum, unit_id, cost, profit, price)" +
                      "values (@billId, @prodId, @Count, @Sum, @unitId, @cost, @profit, @price)";

            using (var db = new NpgsqlConnection(conn))
            {
                db.Open();

                await db.ExecuteAsync(insert);
                var billDal = await GetBillByNumber(nextBillNumber, bill.report_date);

                foreach (var sale in bill.Sales)
                {
                    await db.ExecuteAsync(sql, new { billId = billDal.id, prodId = sale.prod_id, Count = sale.count,
                        Sum = sale.sum, unitId = isNotNull(sale.unit_id), cost = isNotNull(sale.cost), profit = isNotNull(sale.profit),
                        price = isNotNull(sale.price)
                    });
                }
                return billDal.id;
            }
        }

        public async Task<Bill> GetBillByNumber(int billNumber, DateTime reportDate)
        {
            var sql = "select * from \"Bills\" where bill_number = " + billNumber + " and report_date = '" +
                reportDate.ToString("MM.dd.yyyy HH:mm:ss") + "'";
            using (var db = new NpgsqlConnection(conn))
            {
                db.Open();
                return await db.QueryFirstOrDefaultAsync<Bill>(sql);
            }
        }

        public async Task<IEnumerable<Bill>> GetBillsWithSales(int shopId, DateTime from, DateTime to)
        {
            var join = "select * from \"Bills\" as b join \"Sales\" as s on b.id = s.bill_id where b.shop_id = " + 
                shopId + " and b.report_date between '" + from.ToString("MM.dd.yyyy HH:mm:ss") + "' and '" 
                + to.ToString("MM.dd.yyyy HH:mm:ss") + "'";
            var prodSql = "select * from \"Products\" where id = @ProdId";
            var priceSelect = "select * from \"Prices\" where prod_id = @ProdId";

            using (var db = new NpgsqlConnection(conn))
            {
                db.Open();
                var billDict = new Dictionary<int, Bill>();

                var bills = (await db.QueryAsync<Bill, Sale, Bill>(join, 
                    (bill, sales) =>
                    {
                        Bill billEntry;
                        if (!billDict.TryGetValue(bill.id, out billEntry))
                        {
                            billEntry = bill;
                            billEntry.Sales = new List<Sale>();
                            billDict.Add(billEntry.id, billEntry);
                        }

                        billEntry.Sales.Add(sales);
                        return billEntry;
                    },
                    splitOn: "id")).Distinct().ToList();
                foreach (var bill in bills)
                {
                    foreach (var sale in bill.Sales)
                    {
                        sale.Product = await db.QueryFirstOrDefaultAsync<Product>(prodSql, new { ProdId = sale.prod_id });
                        if (sale.Product != null)
                            sale.Product.Price = await db.QueryFirstOrDefaultAsync<Price>(priceSelect, new { ProdId = sale.prod_id });
                    }
                }
                return bills;
            }

        }
        
        public async Task<Bill> GetByIdAsync(int id)
        {
            var sql = "select * from \"Bills\" where id = " + id;
            using(var db = new NpgsqlConnection(conn))
            {
                db.Open();
                return await db.QueryFirstOrDefaultAsync<Bill>(sql);
            }
        }

        private async Task<int> GetMaxBillNumberInDay(int shopId, DateTime day)
        {
            var from = new DateTime(day.Year, day.Month, day.Day);
            var to = new DateTime(day.Year, day.Month, day.Day + 1).AddSeconds(-1);
            var sql = "select bill_number from \"Bills\" where shop_id = " + shopId + " and report_date between '" +
                      from.ToString("MM.dd.yyyy HH:mm:ss") + "' and '"
                      + to.ToString("MM.dd.yyyy HH:mm:ss") + "'";

            var bills = await _unitOfWork.Connection.QueryAsync<int>(sql, transaction:_unitOfWork.Transaction);
            if (bills == null || !bills.Any())
            {
                return 1;
            }

            return bills.Max() + 1;

        }
    }
}
