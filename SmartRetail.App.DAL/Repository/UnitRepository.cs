using System.Collections.Generic;
using System.Data.SqlClient;
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
        
        public IEnumerable<Units> GetAllUnits()
        {
            var sql = "select * from Units";
            using (var conn = new SqlConnection(connection))
            {
                return conn.Query<Units>(sql);
            }
        }
    }
}