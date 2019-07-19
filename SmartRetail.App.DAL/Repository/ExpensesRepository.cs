using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using SmartRetail.App.DAL.Entities;

namespace SmartRetail.App.DAL.Repository
{
    public class ExpensesRepository: IExpensesRepository
    {
        private readonly string _connectionString;

        public ExpensesRepository(string conn)
        {
            _connectionString = conn;
        }
        
        public IEnumerable<Expenses> GetExpenses(int businessId, int? shopId, DateTime from, DateTime to)
        {
            var sql = "";
            if (shopId.HasValue)
            {
                sql = "SELECT * FROM Expenses WHERE business_id = @BusinessId AND shop_id = @ShopId and report_date between '" + from.ToString("MM.dd.yyyy") + "' and '" + to.ToString("MM.dd.yyyy") + "'";
            }
            else
            {
                sql = "SELECT * FROM Expenses WHERE business_id = @BusinessId and report_date between '" + from.ToString("MM.dd.yyyy") + "' and '" + to.ToString("MM.dd.yyyy") + "'";
            }

            var subSql = "select * from ExpensesType where id = @TypeId";
            
            using (var connection = new SqlConnection(_connectionString))
            {
                var exps = connection.Query<Expenses>(sql, new {ShopId = shopId, BusinessId = businessId});
                foreach (var exp in exps)
                {
                    exp.ExpensesType = connection.Query<ExpensesType>(subSql, new { TypeId = exp.type_id}).FirstOrDefault();
                }

                return exps;
            }
        }
    }
}