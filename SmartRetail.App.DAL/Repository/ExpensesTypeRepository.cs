using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Repository.Interfaces;

namespace SmartRetail.App.DAL.Repository
{
    public class ExpensesTypeRepository: EntityRepository<ExpensesType>, IExpensesTypeRepository
    {
        private readonly string conn;
        
        public ExpensesTypeRepository(string connection)
        {
            conn = connection;
        }

        public new IEnumerable<ExpensesType> GetAll()
        {
            var sql = "SELECT * FROM ExpensesType";
            using (var connection = new SqlConnection(conn))
            {
                return connection.Query<ExpensesType>(sql);
            }
        }

    }
}
