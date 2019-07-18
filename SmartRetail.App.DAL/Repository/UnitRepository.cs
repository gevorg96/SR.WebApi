using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using SmartRetail.App.DAL.Entities;

namespace SmartRetail.App.DAL.Repository
{
    public class UnitRepository: EntityRepository<Units>, IUnitRepository
    {
        private readonly string connection;
        public UnitRepository(string connection)
        {
            this.connection = connection;
        }
        
        public async Task<IEnumerable<Units>> GetAllUnitsAsync()
        {
            var sql = "select * from Units";
            using (var conn = new SqlConnection(connection))
            {
                return await conn.QueryAsync<Units>(sql);
            }
        }
    }
}