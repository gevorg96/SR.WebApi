using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Repository.Interfaces;
using SmartRetail.App.DAL.UnitOfWork;

namespace SmartRetail.App.DAL.Repository
{
    public class BusinessRepository: IBusinessRepository
    {
        private readonly string _connectionString;
        private IUnitOfWork _unitOfWork;

        public BusinessRepository(string conn)
        {
            _connectionString = conn;
        }

        public BusinessRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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

        public async Task<Business> GetByIdUow(int businessId)
        {
            var sql = "SELECT * FROM Business WHERE id = @BusinessId";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<Business>(sql, new { BusinessId = businessId }, _unitOfWork.Transaction);
        }

        public async Task<Business> GetByIdAsync(int businessId)
        {
            var sql = "SELECT * FROM Business WHERE id = @BusinessId";
            using (var db = new SqlConnection(_connectionString))
            {
                return await db.QueryFirstOrDefaultAsync<Business>(sql, new {BusinessId = businessId});
            }
        }
    }
}