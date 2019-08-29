using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Helpers;

namespace SmartRetail.App.DAL.Repository.Interfaces
{
    public interface IFoldersRepository
    {
        Task<Folders> GetByIdAsync(int id);
        Task<IEnumerable<Folders>> GetByBusinessAsync(int businessId);
        Task<IEnumerable<Folders>> GetSubTreeAsync(int rootId);
        Task AddFolderAsync(Folders folder);
        Task AddFolderSubTreeAsync(Tree<Folders> foldersTree);
        Task UpdateFolderAsync(Folders folder);
        Task DeleteFolderAsync(int folderId);
    }
}
