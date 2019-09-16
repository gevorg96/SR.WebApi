using System.Collections.Generic;
using System.Threading.Tasks;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Helpers;

namespace SmartRetail.App.DAL.Repository.Interfaces
{
    public interface IFoldersRepository
    {
        Task<IEnumerable<Folder>> GetPathByChildId(int id);
        Task<IEnumerable<Folder>> GetByBusinessAsync(int businessId);
        Task<Tree<Folder>> GetSubTreeAsync(int rootId);
        Task AddFolderSubTreeAsync(Tree<Folder> foldersTree);
        Task UpdateFolderAsync(Folder folder);
        Task DeleteFoldersAsync(Tree<Folder> tree);
    }
}
