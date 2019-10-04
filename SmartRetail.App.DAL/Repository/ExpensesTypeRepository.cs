using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Repository.Interfaces;

namespace SmartRetail.App.DAL.Repository
{
    public class ExpensesTypeRepository: IExpensesTypeRepository
    {
        private readonly string conn;
        
        public ExpensesTypeRepository(string connection)
        {
            conn = connection;
        }

        public IEnumerable<ExpensesType> GetAll()
        {
            var sql = "SELECT * FROM ExpensesTypes";
            using (var connection = new SqlConnection(conn))
            {
                return connection.Query<ExpensesType>(sql);
            }
        }

        public async Task<IEnumerable<ExpensesType>> GetAllAsync()
        {
            var sql = "SELECT * FROM ExpensesTypes";
            using (var connection = new SqlConnection(conn))
            {
                return await connection.QueryAsync<ExpensesType>(sql);
            }
        }
    }
}
