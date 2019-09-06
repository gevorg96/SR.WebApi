using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Repository.Interfaces;
using Dapper;
using SmartRetail.App.DAL.Helpers;
using static SmartRetail.App.DAL.Helpers.NullChecker;

namespace SmartRetail.App.DAL.Repository
{
    public class FoldersRepository: IFoldersRepository
    {
        private readonly string conn;

        public FoldersRepository(string conn)
        {
            this.conn = conn;
        }

        public async Task<Folders> GetByIdAsync(int id)
        {
            var sql = "select * from Folders where id = " + id;

            using (var db = new SqlConnection(conn))
            {
                db.Open();
                return await db.QueryFirstOrDefaultAsync<Folders>(sql);
            }
        }

        public async Task<IEnumerable<Folders>> GetByBusinessAsync(int businessId)
        {
            var sql = "select * from Folders where business_id = " + businessId;

            using (var db = new SqlConnection(conn))
            {
                db.Open();
                return await db.QueryAsync<Folders>(sql);
            }
        }

        public Task<IEnumerable<Folders>> GetSubTreeAsync(int rootId)
        {
            throw new NotImplementedException();
        }

        public async Task AddFolderAsync(Folders folder)
        {
            var sql = "insert into Folders (business_id, parent_id, folder) values ("+ folder.business_id +", " + isNotNull( folder.parent_id) + ", N'" + folder.folder + ")";
            using (var db = new SqlConnection(conn))
            {
                db.Open();
                await db.ExecuteAsync(sql);
            }

        }

        public async Task AddFolderSubTreeAsync(Tree<Folders> foldersTree)
        { 
            using (var db = new SqlConnection(conn))
            {
                db.Open();
                await InsertChilds(foldersTree.Parent?.Value?.id, foldersTree, db);
            }
        }

        private async Task InsertChilds(int? parentId , Tree<Folders> foldersTree, SqlConnection db)
        {
            if (isFile(foldersTree.Value.folder))
            {
                var prodIdStr = foldersTree.Value.folder.Split('.').FirstOrDefault();
                if (!string.IsNullOrEmpty(prodIdStr))
                {
                    var prodId = Int32.Parse(prodIdStr);
                    var sql = "update Product set folder_id = " + isNotNull(parentId) + " where id = " + prodId;
                    await db.ExecuteAsync(sql);
                    return;
                }
            }

            var insertSql = "insert into Folders (business_id, parent_id, folder) values (" + foldersTree.Value.business_id + ", " +
                            isNotNull(parentId)+", N'" + foldersTree.Value.folder + "')";
            var selectSql = "select * from Folders where business_id = " + foldersTree.Value.business_id +
                            " and parent_id " + (parentId.HasValue? " = " + parentId.Value  : "is null") + " and  folder = N'" + foldersTree.Value.folder+"'";
            if (foldersTree != null)
            {
                await db.ExecuteAsync(insertSql, new {parent = parentId, folder = foldersTree.Value.folder});
                var dal = await db.QueryFirstOrDefaultAsync<Folders>(selectSql,
                    new { foldersTree.Value.folder, parent = isNotNull(parentId) });
                if (foldersTree.Children != null && foldersTree.Children.Any())
                {
                    foreach (var child in foldersTree.Children)
                    {
                        await InsertChilds(dal.id, child, db);
                    }
                }
            }
        }
        public static bool isFile(string value)
        {
            var parts = value.Split('.');
            var format = parts[parts.Length - 1].ToUpperInvariant();
            switch (format)
            {

                case "JPG":
                case "PNG":
                case "JPEG":
                case "RAW":
                case "TIFF":
                case "BMP":
                case "GIF":
                case "JP2":
                case "PCX":
                case "ICO":
                    return true;
                default:
                    return false;
            }
        }


        public Task UpdateFolderAsync(Folders folder)
        {
            throw new NotImplementedException();
        }

        public Task DeleteFolderAsync(int folderId)
        {
            throw new NotImplementedException();
        }
    }
}
