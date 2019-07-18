using Dapper;
using SmartRetail.App.DAL.Entities;
using System.Data.SqlClient;

namespace SmartRetail.App.DAL.Repository
{
    public class UserRepository : EntityRepository<UserProfile>, IUserRepository
    {
        private string connectionString;

        public UserRepository(string conn)
        {
            connectionString = conn;
        }

        public new void Add(UserProfile entity)
        {
            string sql = "INSERT INTO UserProfile (UserName, Email, Password, shop_id, business_id, access_grade) Values (@UserName, @Email, @Password, @ShopId, @BusinessId, @AccessGrade);";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Execute(sql, new { UserName = entity.UserName, Email = entity.Email, Password = entity.Password, ShopId = entity.shop_id, BusinessId = entity.business_id , AccessGrade = entity.access_grade});
            }
        }

        public new UserProfile GetById(int id)
        {
            string sql = "SELECT * FROM UserProfile WHERE UserId = @Id";
            using (var connection = new SqlConnection(connectionString))
            {
                return connection.QueryFirst<UserProfile>(sql, new { Id = id });
            }
        }

        public UserProfile GetByLogin(string login)
        {
            string sql = "SELECT * FROM UserProfile WHERE UserName = @Login";
            using (var connection = new SqlConnection(connectionString))
            {
                return connection.QueryFirst<UserProfile>(sql, new { Login = login });
            }
        }

        public void Update(UserProfile entity, string field, string value)
        {
            string sql = string.Format("UPDATE UserProfile SET {0} = '{1}' WHERE UserId = {2};", field, value, entity.UserId);

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Execute(sql);
            }
        }
    }
}
