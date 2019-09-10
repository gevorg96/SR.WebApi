using System.Collections.Generic;
using System.Threading.Tasks;
using SmartRetail.App.DAL.BLL.HelperClasses;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Helpers;

namespace SmartRetail.App.DAL.BLL.DataServices
{
    public interface IFoldersDataService
    {
        Task<Tree<ImgTwinModel>> GetTreeAsync(int businessId);
        Task<Tree<ImgTwinModel>> GetFoldersTreeAsync(int businessId);
        IEnumerable<ImgTwinModel> GetLevel(string fullPath);
        Tree<ImgTwinModel> SearchSubTree(string fullPath);
        IEnumerable<ImgTwinModel> Search(Tree<ImgTwinModel> treePart, string search);
        Tree<ImgTwinModel> Tree { get; }

        Task<ImgTwinModel> GetById(int id, int businessId); 
        Task<int?> GetFolderIdByPath(string path, int businessId);
        Task AddFoldersByPath(string path, int businessId);
        Task RenameFolderByPath(string path, string newName, int businessId);
        Task ReplaceFolderByPath(string oldPath, string newPath, int businessId, bool copy = false);
        Task DeleteFolderById(int id);
    }
}
