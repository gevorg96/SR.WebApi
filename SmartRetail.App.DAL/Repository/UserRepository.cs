using Dapper;
using SmartRetail.App.DAL.Entities;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Npgsql;
using SmartRetail.App.DAL.Repository.Interfaces;

namespace SmartRetail.App.DAL.Repository
{
    public class UserRepository : IUserRepository
    {
        private string connectionString;

        public UserRepository(string conn)
        {
            connectionString = conn;
        }

        #region Create

        public void Add(UserProfile entity)
        {
            string sql = "INSERT INTO \"UserProfiles\" (UserName, Email, Password, shop_id, business_id, access_grade) Values (@UserName, @Email, @Password, @ShopId, @BusinessId, @AccessGrade);";

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                connection.Execute(sql, new { UserName = entity.UserName, Email = entity.Email, Password = entity.Password, ShopId = entity.shop_id, BusinessId = entity.business_id , AccessGrade = entity.access_grade});
            }
        }

        #endregion

        #region Read

        public UserProfile GetById(int id)
        {
            string sql = "SELECT * FROM \"UserProfiles\" WHERE UserId = @Id";
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                return connection.QueryFirst<UserProfile>(sql, new { Id = id });
            }
        }

        public async Task<UserProfile> GetByLogin(string login)
        {
            string sql = "SELECT * FROM \"UserProfiles\" WHERE UserName = @Login";
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                return await connection.QueryFirstOrDefaultAsync<UserProfile>(sql, new { Login = login });
            }
        }

        #endregion
    }
}
