using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartRetail.App.DAL.BLL.HelperClasses;
using SmartRetail.App.DAL.BLL.StructureFillers;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Helpers;
using SmartRetail.App.DAL.Repository;
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
            this._foldersRepo = foldersRepo;
            this._productRepo = productRepo;
            _categoryFiller = new CategoryTreeFiller(foldersRepo);
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

        public async Task<int?> GetFolderIdByPath(string path, int businessId)
        {
            var folders = await _foldersRepo.GetByBusinessAsync(businessId);
            var pathParts = path.Split('/').Where(p => !string.IsNullOrEmpty(p));
            var (index, parent) = GetParentFolder(pathParts, folders);
            
            return parent?.id;
        }

        public async Task AddFoldersByPath(string path, int businessId)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            var folders = await _foldersRepo.GetByBusinessAsync(businessId);
            var pathParts = path.Split('/').Where(p => !string.IsNullOrEmpty(p));
            var (index, parent)= GetParentFolder(pathParts, folders);
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
    }
}
