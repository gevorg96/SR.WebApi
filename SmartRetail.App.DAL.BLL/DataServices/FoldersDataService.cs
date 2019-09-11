using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartRetail.App.DAL.BLL.HelperClasses;
using SmartRetail.App.DAL.BLL.StructureFillers;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Helpers;
using SmartRetail.App.DAL.Repository.Interfaces;

namespace SmartRetail.App.DAL.BLL.DataServices
{
    public class FoldersDataService : IFoldersDataService
    {
        private readonly IFoldersRepository _foldersRepo;
        private readonly IProductRepository _productRepo;
        private readonly CategoryTreeFiller _categoryFiller;
        public Tree<ImgTwinModel> Tree { get; private set; }


        public FoldersDataService(IFoldersRepository foldersRepo, IProductRepository productRepo)
        {
            _foldersRepo = foldersRepo;
            _productRepo = productRepo;
            _categoryFiller = new CategoryTreeFiller();
        }

        public async Task<Tree<ImgTwinModel>> GetTreeAsync(int businessId)
        {
            var foldersTask = Task.Run(() => _foldersRepo.GetByBusinessAsync(businessId));
            var productsTask = Task.Run(() => _productRepo.GetProductsByBusinessAsync(businessId));
            var (folders, products) = await Tasker.WhenAll(foldersTask, productsTask);

            Tree = _categoryFiller.CreateTree(folders.ToList(), products.ToList());
            return Tree;
        }

        public async Task<Tree<ImgTwinModel>> GetFoldersTreeAsync(int businessId)
        {
            var folders = await _foldersRepo.GetByBusinessAsync(businessId);

            return _categoryFiller.CreateTreeFolders(folders);
        }


        public IEnumerable<ImgTwinModel> GetLevel(string fullPath)
        {
            return _categoryFiller.GetLevel(fullPath);
        }

        public Tree<ImgTwinModel> SearchSubTree(string fullPath)
        {
            return _categoryFiller.SearchSubTree(fullPath);
        }

        public IEnumerable<ImgTwinModel> Search(Tree<ImgTwinModel> treePart, string search)
        {
            return CategoryTreeFiller.Search(treePart, search);
        }

        public async Task<ImgTwinModel> GetById(int id, int businessId)
        {
            var path = await _foldersRepo.GetPathByChildId(id);
            var sb = new StringBuilder();
            foreach (var folder in path)
            {
                sb.Append("/" + folder.folder);
            }

            var child = path.LastOrDefault();
            if (child == null)
            {
                return null;
            }

            return new ImgTwinModel
            {
                folder = child.folder,
                id = child.id,
                fullpath = sb.ToString(),
                isFile = false
            };

        }
        
        public async Task<int?> GetFolderIdByPath(string path, int businessId)
        {
            var folders = await _foldersRepo.GetByBusinessAsync(businessId);
            var pathParts = path.Split('/').Where(p => !string.IsNullOrEmpty(p));
            var (index, parent) = GetParentFolder(pathParts, folders);
            
            return parent?.id;
        }

        public async Task AddFoldersByPath(string path, int businessId)
        {
            var (index, parent, pathParts) = await ComplexSearchByPath(path, businessId);
            var tree = new Tree<Folders>
            {
                Value = new Folders
                {
                    business_id = businessId,
                    folder = pathParts.ElementAt(index)
                },
                Parent = null
            };

            FillTreeByPath(pathParts, index+1, tree);
            tree.Parent = new Tree<Folders> { Value = parent };
            await _foldersRepo.AddFolderSubTreeAsync(tree);
        }

        public async Task RenameFolderByPath(string path, string newName, int businessId)
        {
            var (index, parent, pathParts) = await ComplexSearchByPath(path, businessId);
            if (index != pathParts.Count())
            {
                throw new Exception("Нет такой папки по данному пути.");
            }

            parent.folder = newName;
            await _foldersRepo.UpdateFolderAsync(parent);

        }

        public async Task ReplaceFolderByPath(string oldPath, string newPath, int businessId, bool copy = false)
        {
            var (index, parent, pathParts) = await ComplexSearchByPath(oldPath, businessId);
            if (index != pathParts.Count())
            {
                throw new Exception("Нет такой папки по данному пути.");
            }

            var (newIndex, newParent, newPathParts) = await ComplexSearchByPath(newPath, businessId);
            if (newIndex != newPathParts.Count())
            {
                throw new Exception("Нет такой папки по данному пути.");
            }

            if (!copy)
            {
                parent.parent_id = newParent.id;
                await _foldersRepo.UpdateFolderAsync(parent);
            }
            else
            {
                var subTree = await _foldersRepo.GetSubTreeAsync(parent.id);
                subTree.Parent = new Tree<Folders>{Value = newParent};
                subTree.Value.id = 0;
                subTree.Value.parent_id = 0;
                NullifyIds(subTree);
                await _foldersRepo.AddFolderSubTreeAsync(subTree);
            }
        }

        public async Task DeleteFolderById(int id)
        {
            var subTree = await _foldersRepo.GetSubTreeAsync(id);
            await _foldersRepo.DeleteFoldersAsync(subTree);
        }

        private (int, Folders) GetParentFolder(IEnumerable<string> pathParts, IEnumerable<Folders> folders)
        {
            if (pathParts == null || folders == null)
            {
                return (0, null);
            }

            var memory = new List<Folders> {folders.FirstOrDefault(p => p.folder == pathParts.FirstOrDefault())};
            int index;

            for (index = 1; index < pathParts.Count(); index++)
            {
                var part = pathParts.ElementAt(index);
                var target = folders.FirstOrDefault(p => p.folder == part && p.parent_id == memory[index - 1].id);
                if (target != null)
                {
                    memory.Add(target);
                }
                else
                {
                    break;
                }
            }
            return (index, memory.LastOrDefault());
        }

        private void FillTreeByPath(IEnumerable<string> pathParts, int index, Tree<Folders> tree)
        {
            if (index >= pathParts.Count())
            {
                return;
            }

            var child = tree.AddChild(new Folders
            {
                folder = pathParts.ElementAt(index),
                business_id = tree.Value.business_id
            });
            FillTreeByPath(pathParts, index + 1, child);
        }

        private async Task<(int, Folders, IEnumerable<string>)> ComplexSearchByPath(string path, int businessId)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new Exception("Пустая строка.");
            }

            var folders = await _foldersRepo.GetByBusinessAsync(businessId);
            var pathParts = path.Split('/').Where(p => !string.IsNullOrEmpty(p));
            var (index, parent) =  GetParentFolder(pathParts, folders);
            return (index, parent, pathParts);
        }

        private void NullifyIds(Tree<Folders> tree)
        {
            foreach (var child in tree.Children)
            {
                child.Value.id = 0;
                child.Value.parent_id = 0;
                NullifyIds(child);
            }
        }
    }
}
