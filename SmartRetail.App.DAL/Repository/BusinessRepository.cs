using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Helpers;

namespace SmartRetail.App.DAL.Repository
{
    public class BusinessRepository: IBusinessRepository
    {
        private readonly string _connectionString;

        public BusinessRepository(string conn)
        {
            _connectionString = conn;
        }

        public IEnumerable<Business> GetAll()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open();
                return db.Query<Business>(QueryHelper.GetBasicQuery(null,"Business")).ToList();
            }
        }

        public IEnumerable<Business> GetWithFilter(string field, string value)
        {
            var sql = "SELECT * FROM Business WHERE " + field + " = " + value; 
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open();
                return db.Query<Business>(sql).ToList();

            }
        }

        public void Add(Business b)
        {
            var sql = "insert into Business (name, tel) values (@name, @tel})"; 
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open();
                db.Execute(sql, new { name = b.name, tel = b.tel});
            }
        }

        public Business GetById(int id)
        {
            var sql = "SELECT * FROM Business WHERE id = @BusinessId"; 
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open();
                return db.Query<Business>(sql, new {BusinessId = id}).FirstOrDefault();
            }
        }

        public async Task<Business> GetByIdAsync(int businessId)
        {
            var sql = "SELECT * FROM Business WHERE id = @BusinessId";
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open();
                return await db.QueryFirstOrDefaultAsync<Business>(sql, new { BusinessId = businessId });
            }
        }
    }
}