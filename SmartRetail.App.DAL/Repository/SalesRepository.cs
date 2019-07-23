using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SmartRetail.App.DAL.Entities;
using static SmartRetail.App.DAL.Helpers.NullChecker;

namespace SmartRetail.App.DAL.Repository
{
    public class SalesRepository : ISalesRepository
    {
        private readonly string conn;

        public SalesRepository(string conn)
        {
            this.conn = conn;
        }

        #region Read

        public IEnumerable<Sales> GetSalesByShopAndReportDate(int shopId, DateTime from, DateTime to)
        {
            var dtStart = from;
            var dtEnd = to;

            var sql = "select * from Sales where shop_id = " + shopId +" and report_date between '" + from.ToString("MM.dd.yyyy HH:mm:ss") + "' and '" + dtEnd.ToString("MM.dd.yyyy HH:mm:ss")  +"'";
            var subSql = "select * from Product where id = @ProdId";
            var priceSelect = "select * from Price where prod_id = @ProdId";

            using (var connection = new SqlConnection(conn))
            {
                connection.Open();
                var sales = connection.Query<Sales>(sql, new {ShopId = shopId});

                foreach (var sale in sales)
                {
                    sale.Product = connection.Query<Product>(subSql, new { ProdId = sale.prod_id }).FirstOrDefault();
                    if (sale.Product != null)
                        sale.Product.Price = connection.Query<Price>(priceSelect, new {ProdId = sale.prod_id}).FirstOrDefault();
                }

                return sales;
            }
        }

        #endregion

        #region Create

        public async Task AddSalesAsync(Sales sales)
        {
            var sql = "INSERT INTO Sales (prod_id, shop_id, report_date, bill_number, summ, sales_count, unit_id)" +
                      "values ( " + isNotNull(sales.prod_id) + ", " + isNotNull(sales.shop_id) + ", '" +
                      sales.report_date.Value.ToString("MM.dd.yyyy hh:mm:ss") + "', " + isNotNull(sales.bill_number) +
                ", " + isNotNull(sales.summ) + ", " + isNotNull(sales.sales_count) + ", " + isNotNull(sales.unit_id) + ")";

            using (var connection = new SqlConnection(conn))
            {
                connection.Open();
                await connection.ExecuteAsync(sql);
            }
        }

        public async Task<IEnumerable<Sales>> GetSalesByProdIdAndBill(int billNumber, int prodId)
        {
            var select = "select * from Sales where bill_number = " + billNumber + " and prod_id = " + prodId;
            using (var db = new SqlConnection(conn))
            {
                db.Open();
                return await db.QueryAsync<Sales>(select);
            }
        }

        #endregion
    }
}
