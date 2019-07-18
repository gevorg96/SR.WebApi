using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Helpers;

namespace SmartRetail.App.DAL.Repository
{
    public class BusinessRepository: EntityRepository<Business>, IBusinessRepository
    {
        private readonly string _connectionString;

        public BusinessRepository(string conn)
        {
            _connectionString = conn;
        }

        public new IEnumerable<Business> GetAll()
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

        public new void Add(Business b)
        {
            var sql = "insert into Business (name, tel) values (@name, @tel})"; 
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open();
                db.Execute(sql, new { name = b.name, tel = b.tel});
            }
        }

        public new Business GetById(int id)
        {
            var sql = "SELECT * FROM Business WHERE id = @BusinessId"; 
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open();
                return db.Query<Business>(sql, new {BusinessId = id}).FirstOrDefault();
            }
        }
    }
}