using System.Collections.Generic;
using System.Threading.Tasks;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Helpers;

namespace SmartRetail.App.DAL.Repository.Interfaces
{
    public interface IFoldersRepository
    {
        Task<IEnumerable<Folders>> GetPathByChildId(int id);
        Task<IEnumerable<Folders>> GetByBusinessAsync(int businessId);
        Task<Tree<Folders>> GetSubTreeAsync(int rootId);
        Task AddFolderSubTreeAsync(Tree<Folders> foldersTree);
        Task UpdateFolderAsync(Folders folder);
        Task DeleteFoldersAsync(Tree<Folders> tree);
    }
}
