﻿using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Helpers;
using SmartRetail.App.DAL.Repository.Interfaces;
using static SmartRetail.App.DAL.Helpers.NullChecker;

namespace SmartRetail.App.DAL.Repository
{
    public class ShopRepository: IShopRepository
    {
        private readonly string _connectionString;
        private QueryBuilder qb;

        public ShopRepository(string conn)
        {
            _connectionString = conn;
            qb = new QueryBuilder();
        }

        #region Read

        public IEnumerable<Shop> GetShopsByBusiness(int businessId)
        {
            string sql = "select * from \"Shops\" where business_id = @BusinessId";
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                return connection.Query<Shop>(sql, new {BusinessId = businessId});
            }
        }

        public Shop GetById(int shopId)
        {
            var sql = "SELECT * FROM \"Shops\" WHERE id = @ShopId";
            using (IDbConnection db = new NpgsqlConnection(_connectionString))
            {
                db.Open();
                return db.Query<Shop>(sql, new {ShopId = shopId}).FirstOrDefault();
            }
        }

        #endregion

        #region Create

        public async Task AddAsync(Shop entity)
        {
            var sql = "insert into \"Shops\" (business_id, shop_address, name) values (" + isNotNull(entity.business_id) + 
                ", " + isNotNull(entity.shop_address) + ", " + isNotNull(entity.name) + ");";
            using(var db = new NpgsqlConnection(_connectionString))
            {
                db.Open();
                var affRows = await db.ExecuteAsync(sql);
            }
        }

        #endregion

        #region Update

        public async Task UpdateAsync(Shop entity)
        {
            qb.Clear();
            var select = qb.Select("*").From("\"Shops\"").Where("id").Op(Ops.Equals, entity.id.ToString());
            var sb = new StringBuilder();
            sb.Append("update \"Shops\" set ");

            var pi = entity.GetType().GetProperties();

            using (var db = new NpgsqlConnection(_connectionString))
            {
                db.Open();
                var row = db.QueryFirstOrDefault<Shop>(select.ToString());
                for (var i = 1; i < 4; i++)
                {
                    var p = pi[i];
                    var pt = p.PropertyType.ToString();
                    var newValue = p.GetValue(entity);
                    var oldValue = p.GetValue(row);
                    if (newValue != null && (oldValue == null || newValue.ToString() != oldValue.ToString()))
                    {
                        sb.Append(p.Name + " = " + QueryHelper.GetSqlString(p, p.GetValue(entity)) + ", ");
                    }
                }
                if (sb.Length < 20) return;

                sb.Remove(sb.Length - 2, 2);
                sb.Append(" where id = " + entity.id);
                var rows = await db.ExecuteAsync(sb.ToString());
            }
        }

        #endregion
    }
}
