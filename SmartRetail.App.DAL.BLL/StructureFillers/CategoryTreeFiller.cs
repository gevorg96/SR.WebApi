using System.Collections.Generic;
using System.Linq;
using SmartRetail.App.DAL.BLL.HelperClasses;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Helpers;
using SmartRetail.App.DAL.Repository.Interfaces;

namespace SmartRetail.App.DAL.BLL.StructureFillers
{
    public class CategoryTreeFiller: ICategoryTreeFiller
    {
        private readonly IFoldersRepository _foldersRepo;
        private Tree<ImgTwinModel> _tree;

        public CategoryTreeFiller(IFoldersRepository foldersRepo)
        {
            _foldersRepo = foldersRepo;
        }
        public Tree<ImgTwinModel> CreateTree(IEnumerable<Folders> folders, IEnumerable<Product> products)
        {
            var root = folders.FirstOrDefault(p => p.parent_id == null);
            if (root == null)
            {
                return null;
            }
            _tree = new Tree<ImgTwinModel>
            {
                Value = new ImgTwinModel {id = root.id, folder = root.folder, isFile = false, fullpath = "/" + root.folder},
                Parent = null
            };
            FillNextLevel(folders, products, _tree, root.id);
            return _tree;
        }

        public Tree<ImgTwinModel> CreateTreeFolders(IEnumerable<Folders> folders)
        {
            var root = folders.FirstOrDefault(p => p.parent_id == null);
            if (root == null)
            {
                return null;
            }
            var tree = new Tree<ImgTwinModel>
            {
                Value = new ImgTwinModel { id = root.id, folder = root.folder, isFile = false, fullpath = "/" + root.folder },
                Parent = null
            };
            FillNextLevel(folders, null, tree, root.id);
            return tree;
        }

        public IEnumerable<ImgTwinModel> GetLevel(string fullPath)
        {
            if (_tree == null)
                return null;
            
            return Tree<ImgTwinModel>.Search(_tree, new ImgTwinModel { fullpath = fullPath })
                .Children.Select(p => new ImgTwinModel {id = p.Value.id, folder = p.Value.folder, fullpath = p.Value.fullpath, isFile = isFile(p.Value.folder) }).ToList();
        }

        public Tree<ImgTwinModel> SearchSubTree(string fullPath)
        {
            return _tree == null ? null : Tree<ImgTwinModel>.Search(_tree, new ImgTwinModel { fullpath = fullPath });
        }

        public static IEnumerable<ImgTwinModel> Search(Tree<ImgTwinModel> treePart, string search)
        {
            if (treePart == null)
                return null;
            var result = new List<ImgTwinModel>();
            var treeList = Tree<ImgTwinModel>.ToList(treePart);

            foreach (var node in treeList)
            {
                if (node.folder.Contains(search))
                {
                    result.Add(node);
                }
            }

            return result.Select(p => new ImgTwinModel {id = p.id, folder = p.folder, fullpath = p.fullpath, isFile = isFile(p.folder) }).ToList(); ;
        }

        private static void FillNextLevel(IEnumerable<Folders> folders, IEnumerable<Product> products, Tree<ImgTwinModel> tree,
            int? parentId)
        {

            var targetFolders = new List<Folders>();
            var targetProducts = new List<Product>();
            if (folders != null && folders.Any())
            {
                targetFolders = folders.Where(p => p.parent_id == parentId).ToList();
            }

            if (products != null && products.Any())
            {
                targetProducts = products.Where(p => p.folder_id == parentId).ToList();
            }
            
            if (targetFolders.Any() || targetProducts.Any())
            {
                foreach (var folder in targetFolders)
                {
                    var child = tree.AddChild(new ImgTwinModel
                    {
                        id = folder.id,
                        folder = folder.folder,
                        isFile = false,
                        fullpath = tree.Value.fullpath + "/" + folder.folder
                    });
                    FillNextLevel(folders, products, child, folder.id);
                }

                foreach (var product in targetProducts)
                {
                    var imgName = product.id + "." + product.name + "." +
                                  (product.Image != null ? product.Image.img_type : "");
                    tree.AddChild(new ImgTwinModel
                    {
                        id = product.id,
                        folder = imgName,
                        isFile = true,
                        fullpath = tree.Value.fullpath + "/" + imgName
                    });
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
    }
}
