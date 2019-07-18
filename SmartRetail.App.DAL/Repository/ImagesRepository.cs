﻿using Dapper;
using SmartRetail.App.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace SmartRetail.App.DAL.Repository
{
    public class ImagesRepository : EntityRepository<Images>, IImageRepository
    {
        private string connectionString;

        public ImagesRepository(string conn)
        {
            connectionString = conn;
        }

        public new void Add(Images entity)
        {
            string sql = "INSERT INTO Images (ROWGUID, prod_id, img_type, img_name, img_url, img_url_temp, img_path) Values (@ROWGUID, @prod_id, @img_type, @img_name, @img_url, @img_url_temp, @img_path);";

            using (var connection = new SqlConnection(connectionString))
            {
                var affectedRows = connection.Execute(sql, new { ROWGUID = Guid.NewGuid(), prod_id = entity.prod_id, img_type = entity.img_type, 
                    img_name = entity.img_name, img_url = entity.img_url , img_url_temp = entity.img_url_temp, img_path = entity.img_path});
            }
        }

        public new Images GetById(int id)
        {
            string sql = "SELECT * FROM Images WHERE prod_id = @Id";
            using (var connection = new SqlConnection(connectionString))
            {
                return connection.Query<Images>(sql, new {Id = id}).First();
            }
        }

        public void UpdateImage(int prodId, string field, string value)
        {
            string sql = "UPDATE Images SET " + field + " = " +  value + " WHERE prod_id = " + prodId;
            using (var conn = new SqlConnection(connectionString))
            {
                var affectedRows = conn.Execute(sql);
            }
        }

        public IEnumerable<Images> GetAllImages()
        {
            string sql = "SELECT * FROM Images";
            using (var conn = new SqlConnection(connectionString))
            {
                return conn.Query<Images>(sql);
            }
        }
    }
}