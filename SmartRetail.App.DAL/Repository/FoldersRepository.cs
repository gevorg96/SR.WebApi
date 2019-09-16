using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
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
        private QueryBuilder qb;

        public FoldersRepository(string conn)
        {
            this.conn = conn;
            qb = new QueryBuilder();
        }


        public async Task<IEnumerable<Folder>> GetPathByChildId(int id)
        {
            var sql = "select * from Folders where id = @Id";
            var folders = new List<Folder>();

            using (var db = new SqlConnection(conn))
            {
                db.Open();
                var folder = await db.QueryFirstOrDefaultAsync<Folder>(sql, new {Id = id});
                folders.Add(folder);

                while (folder.parent_id.HasValue)
                {
                    folder = await db.QueryFirstOrDefaultAsync<Folder>(sql, new {Id = folder.parent_id.Value});
                    folders.Add(folder);
                }

                folders.Reverse();
                return folders;
            }
        }

        public async Task<IEnumerable<Folder>> GetByBusinessAsync(int businessId)
        {
            var sql = "select * from Folders where business_id = " + businessId;

            using (var db = new SqlConnection(conn))
            {
                db.Open();
                return await db.QueryAsync<Folder>(sql);
            }
        }

        public async Task<Tree<Folder>> GetSubTreeAsync(int rootId)
        {
            var tree = new Tree<Folder>();
            var select = "select * from Folders where id = " + rootId;
            using (var db = new SqlConnection(conn))
            {
                db.Open();
                tree.Value = await db.QueryFirstOrDefaultAsync<Folder>(select);
                await FillTree(tree.Value.id, tree, db);
                return tree;
            }
        }

        public async Task AddFolderSubTreeAsync(Tree<Folder> foldersTree)
        { 
            using (var db = new SqlConnection(conn))
            {
                db.Open();
                await InsertChildren(foldersTree.Parent?.Value?.id, foldersTree, db);
            }
        }

        public async Task UpdateFolderAsync(Folder folder)
        {
            qb.Clear();
            var select = qb.Select("*").From("Folders").Where("id").Op(Ops.Equals, folder.id.ToString());
            var sb = new StringBuilder();
            sb.Append("update Folders set ");
            var pi = folder.GetType().GetProperties();

            using (var db = new SqlConnection(conn))
            {
                db.Open();
                var dal = await db.QueryFirstOrDefaultAsync<Folder>(select.ToString());
                for (var i = 1; i < 4; i++)
                {
                    var p = pi[i];
                    var pt = p.PropertyType.ToString();
                    var newValue = p.GetValue(folder);
                    var oldValue = p.GetValue(dal);
                    if (newValue != null && (oldValue == null || newValue.ToString() != oldValue.ToString()))
                    {
                        sb.Append(p.Name + " = " + QueryHelper.GetSqlString(p, p.GetValue(folder)) + ", ");
                    }
                }
                if (sb.Length < 20) return;

                sb.Remove(sb.Length - 2, 2);
                sb.Append(" where id = " + folder.id);
                await db.ExecuteAsync( sb.ToString());
            }
        }

        public async Task DeleteFoldersAsync(Tree<Folder> tree)
        {
            var delete = "delete from Folders where id = @Id";
            var update = "update Product set folder_id = NULL where folder_id = @folderId";
            var list = Tree<Folder>.ToList(tree).OrderByDescending(p => p.parent_id);
            using (var db = new SqlConnection(conn))
            {
                db.Open();
                using (var transaction = db.BeginTransaction())
                {
                    try
                    {
                        foreach (var folder in list)
                        {
                            await db.ExecuteAsync(delete, new { Id = folder.id }, transaction);
                            await db.ExecuteAsync(update, new { folderId = folder.id }, transaction);
                        }
                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        throw new Exception(e.Message);
                    }
                }
            }
        }

        #region Additional Methods

        private async Task FillTree(int parentId, Tree<Folder> tree, SqlConnection db)
        {
            var sql = "select * from Folders where parent_id = " + parentId;
            var children = await db.QueryAsync<Folder>(sql);
            foreach (var child in children)
            {
                var ch = tree.AddChild(child);
                await FillTree(child.id, ch, db);
            }
        }
        private async Task InsertChildren(int? parentId, Tree<Folder> foldersTree, SqlConnection db)
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
                            isNotNull(parentId) + ", N'" + foldersTree.Value.folder + "')";
            var selectSql = "select * from Folders where business_id = " + foldersTree.Value.business_id +
                            " and parent_id " + (parentId.HasValue ? " = " + parentId.Value : "is null") + " and  folder = N'" + foldersTree.Value.folder + "'";
            if (foldersTree != null)
            {
                await db.ExecuteAsync(insertSql, new { parent = parentId, folder = foldersTree.Value.folder });
                var dal = await db.QueryFirstOrDefaultAsync<Folder>(selectSql,
                    new { foldersTree.Value.folder, parent = isNotNull(parentId) });
                if (foldersTree.Children != null && foldersTree.Children.Any())
                {
                    foreach (var child in foldersTree.Children)
                    {
                        await InsertChildren(dal.id, child, db);
                    }
                }
            }
        }
        private static bool isFile(string value)
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

        #endregion
    }
}
