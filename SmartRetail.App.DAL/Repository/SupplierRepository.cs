using System.Data.SqlClient;
using Dapper;
using System.Threading.Tasks;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Repository.Interfaces;
using static SmartRetail.App.DAL.Helpers.NullChecker;

namespace SmartRetail.App.DAL.Repository
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly string conn;
        public SupplierRepository(string connection)
        {
            conn = connection;
        }

        #region Create

        public async Task AddSupplierAsync(Supplier entity)
        {
            var sql = "insert into Supplier (name, org_name, supp_address, tel) values (" + isNotNull(entity.name) + ", "
                + isNotNull(entity.org_name) + ", " + isNotNull(entity.supp_address) + ", " + isNotNull(entity.tel) + ");";
            using (var db = new SqlConnection(conn))
            {
                db.Open();
                var affectedRows = await db.ExecuteAsync(sql);
            }
        }

        #endregion

        #region Read

        public async Task<Supplier> GetByIdAsync(int id)
        {
            var sql = "select * from Supplier where id = " + id;
            using (var db = new SqlConnection(conn))
            {
                db.Open();
                return await db.QueryFirstOrDefaultAsync<Supplier>(sql);
            }
        }

        #endregion
    }
}
