﻿using Dapper;
using SmartRetail.App.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using SmartRetail.App.DAL.Helpers;
using System.Threading.Tasks;
using System.Text;
using SmartRetail.App.DAL.Repository.Interfaces;

namespace SmartRetail.App.DAL.Repository
{
    public class ImagesRepository : IImageRepository
    {
        private string connectionString;

        public ImagesRepository(string conn)
        {
            connectionString = conn;
        }

        public void Add(Images entity)
        {
            string sql = "INSERT INTO Images (ROWGUID, prod_id, img_type, img_name, img_url, img_url_temp, img_path) Values (@ROWGUID, @prod_id, @img_type, @img_name, @img_url, @img_url_temp, @img_path);";

            using (var connection = new SqlConnection(connectionString))
            {
                var affectedRows = connection.Execute(sql, new { ROWGUID = Guid.NewGuid(), prod_id = entity.prod_id, img_type = entity.img_type, 
                    img_name = entity.img_name, img_url = entity.img_url , img_url_temp = entity.img_url_temp, img_path = entity.img_path});
            }
        }

        public async Task<Images> GetByIdAsync(int id)
        {
            string sql = "SELECT * FROM Images WHERE prod_id = @Id";
            using (var connection = new SqlConnection(connectionString))
            {
                return await connection.QueryFirstOrDefaultAsync<Images>(sql, new { Id = id });
            }
        }

        public async Task UpdateImage(Images img)
        {
            var sql = "select * from Images where prod_id = " + img.prod_id;
            var sb = new StringBuilder();
            sb.Append("update Images set ");

            var pi = img.GetType().GetProperties();

            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                var row = await db.QueryFirstOrDefaultAsync<Images>(sql.ToString());
                for (var i = 2; i <= 6; i++)
                {
                    var p = pi[i];
                    var pt = p.PropertyType.ToString();
                    var newValue = p.GetValue(img);
                    var oldValue = p.GetValue(row);
                    if (newValue != null && (oldValue == null || newValue.ToString() != oldValue.ToString()))
                    {
                        sb.Append(p.Name + " = " + QueryHelper.GetSqlString(p, p.GetValue(img)) + ", ");
                    }
                }
                if (sb.Length < 20) return;

                sb.Remove(sb.Length - 2, 2);
                sb.Append(" where prod_id = " + img.prod_id);
                await db.ExecuteAsync(sb.ToString());
            }
        }
    }
}
