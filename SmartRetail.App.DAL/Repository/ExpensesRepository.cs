using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Repository.Interfaces;
using static SmartRetail.App.DAL.Helpers.NullChecker;

namespace SmartRetail.App.DAL.Repository
{
    public class ExpensesRepository: IExpensesRepository
    {
        private readonly string _connectionString;

        public ExpensesRepository(string conn)
        {
            _connectionString = conn;
        }

        public async Task<IEnumerable<Expenses>> GetExpensesAsync(int businessId, int? shopId, DateTime from, DateTime to)
        {
            var sql = "";
            if (shopId.HasValue)
            {
                sql = "select * from Expenses as e join ExpensesDetails as ed on e.id = ed.expenses_id WHERE business_id = " + businessId +
                    " AND shop_id = " + shopId.Value + " and report_date between '" + from.ToString("MM.dd.yyyy HH:mm:ss") + "' and '"
                    + to.ToString("MM.dd.yyyy HH:mm:ss") + "'";
            }
            else
            {
                sql = "select * from Expenses as e join ExpensesDetails as ed on e.id = ed.expenses_id WHERE " +
                    "business_id = " + businessId + " and report_date between '" + from.ToString("MM.dd.yyyy HH:mm:ss") +
                    "' and '" + to.ToString("MM.dd.yyyy HH:mm:ss") + "'";
            }

            var subSql = "select * from ExpensesType where id = @TypeId";

            using (var db = new SqlConnection(_connectionString))
            {
                db.Open();
                var expDict = new Dictionary<int, Expenses>();

                var exps = (await db.QueryAsync<Expenses, ExpensesDetails, Expenses>(sql,
                    (expenses, expDetail) =>
                    {
                        Expenses expEntry;
                        if (!expDict.TryGetValue(expenses.id, out expEntry))
                        {
                            expEntry = expenses;
                            expEntry.ExpensesDetails = new List<ExpensesDetails>();
                            expDict.Add(expEntry.id, expEntry);
                        }

                        expEntry.ExpensesDetails.Add(expDetail);
                        return expEntry;
                    },
                    splitOn: "id")).Distinct().ToList();

                foreach (var exp in exps)
                {
                    foreach (var ed in exp.ExpensesDetails)
                    {
                        ed.ExpensesType = await db.QueryFirstOrDefaultAsync<ExpensesType>(subSql, new { TypeId = ed.expenses_type_id });
                    }
                }
                return exps;
            }
        }

        public async Task<int> AddExpenses(Expenses exp)
        {
            if (exp.ExpensesDetails == null || !exp.ExpensesDetails.Any())
            {
                return -1;
            }
            var expInsert = "insert into Expenses (business_id, shop_id, sum, report_date) values (" + exp.business_id + ", " + isNotNull(exp.shop_id)
                + ", " + isNotNull(exp.sum) + ", '" + exp.report_date.ToString("MM.dd.yyyy HH:mm:ss") + "')";

            var expDetailsInsert = "insert into ExpensesDetails (expenses_id, expenses_type_id, sum) values(@expId, @expTypeId, @expTypeSum)"; 
            using (var db = new SqlConnection(_connectionString))
            {
                db.Open();
                await db.ExecuteAsync(expInsert);
                var expDal = await GetByDateAndShopBusiness(exp.business_id, exp.report_date, exp.shop_id);
                foreach (var detail in exp.ExpensesDetails)
                {
                    await db.ExecuteAsync(expDetailsInsert, new { expId = expDal.id, expTypeId = detail.expenses_type_id, expTypeSum = isNotNull(detail.sum) });
                }
                return expDal.id;
            }
        }

        public async Task<Expenses> GetByDateAndShopBusiness(int businessId, DateTime reportDate, int? shopid = null)
        {
            var sql = new StringBuilder().Append("select * from Expenses where business_id = " + businessId + " and report_date = '" + reportDate.ToString("MM.dd.yyyy HH:mm:ss") + "'");
            if (shopid.HasValue)
            {
                sql.Append(" and shop_id = " + shopid.Value);
            }

            using (var db = new SqlConnection(_connectionString))
            {
                db.Open();
                return (await db.QueryAsync<Expenses>(sql.ToString())).Last();
            }
        }

        public async Task<Expenses> GetByIdAsync(int id)
        {
            var sql = "select * from Expenses as e join ExpensesDetails as ed on e.id = ed.expenses_id WHERE e.id = " + id;
            var subSql = "select * from ExpensesType where id = @TypeId";

            using (var db = new SqlConnection(_connectionString))
            {
                db.Open();
                var expDict = new Dictionary<int, Expenses>();

                var exps = (await db.QueryAsync<Expenses, ExpensesDetails, Expenses>(sql,
                    (expenses, expDetail) =>
                    {
                        Expenses expEntry;
                        if (!expDict.TryGetValue(expenses.id, out expEntry))
                        {
                            expEntry = expenses;
                            expEntry.ExpensesDetails = new List<ExpensesDetails>();
                            expDict.Add(expEntry.id, expEntry);
                        }

                        expEntry.ExpensesDetails.Add(expDetail);
                        return expEntry;
                    },
                    splitOn: "id")).Distinct().ToList();

                foreach (var exp in exps)
                {
                    foreach (var ed in exp.ExpensesDetails)
                    {
                        ed.ExpensesType = await db.QueryFirstOrDefaultAsync<ExpensesType>(subSql, new { TypeId = ed.expenses_type_id });
                    }
                }
                return exps.FirstOrDefault();
            }
        }
    }
}