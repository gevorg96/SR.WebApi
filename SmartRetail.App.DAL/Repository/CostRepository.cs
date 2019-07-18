using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Repository.Interfaces;
using System.Data.SqlClient;
using Dapper;
using System.Linq;
using System.Collections.Generic;

namespace SmartRetail.App.DAL.Repository
{
    public class CostRepository : EntityRepository<Cost>, ICostRepository
    {
        private readonly string conn;
        public CostRepository(string conn)
        {
            this.conn = conn;
        }

        public IEnumerable<Cost> GetByProdId(int prodId)
        {
            var sql = "select * from Cost where prod_id = " + prodId;
            using (var db = new SqlConnection(conn))
            {
                db.Open();
                return db.Query<Cost>(sql);
            }
        }

        public Cost GetByProdAndShopIds(int prodId, int shopId)
        {
            var sql = "select * from Cost where prod_id = " + prodId + " and shop_id = " + shopId;
            using (var db = new SqlConnection(conn))
            {
                db.Open();
                return db.Query<Cost>(sql).FirstOrDefault();
            }
        }

    }
}
