using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Repository.Interfaces;

namespace SmartRetail.App.DAL.Repository
{
    public class UnitRepository:  IUnitRepository
    {
        private readonly string connection;
        public UnitRepository(string connection)
        {
            this.connection = connection;
        }

        #region Read

        public async Task<IEnumerable<Unit>> GetAllUnitsAsync()
        {
            var sql = "select * from Units";
            using (var conn = new SqlConnection(connection))
            {
                return await conn.QueryAsync<Unit>(sql);
            }
        }
        
        #endregion

    }
}