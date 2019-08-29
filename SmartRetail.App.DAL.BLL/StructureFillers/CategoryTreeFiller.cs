using System.Collections.Generic;
using System.Linq;
using SmartRetail.App.DAL.BLL.HelperClasses;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Helpers;
using SmartRetail.App.DAL.Repository.Interfaces;

namespace SmartRetail.App.DAL.BLL.StructureFillers
{
    public class CategoryTreeFiller
    {
        private readonly IFoldersRepository foldersRepo;

        public CategoryTreeFiller(IFoldersRepository foldersRepo)
        {
            this.foldersRepo = foldersRepo;
        }
        public Tree<ImgTwinModel> CreateTree(List<Folders> folders, List<Product> products)
        {
            var root = folders.FirstOrDefault(p => p.parent_id == null);
            if (root == null)
            {
                return null;
            }
            var tree = new Tree<ImgTwinModel>
            {
                Value = new ImgTwinModel {folder = root.folder, isFile = false, fullpath = "/Кайфы от Петерфельдо"},
                Parent = null
            };
            FillNextLevelFolders(folders, products, tree, root.id);
            return tree;
        }

        private void FillNextLevelFolders(List<Folders> folders, List<Product> products, Tree<ImgTwinModel> tree,
            int? parentId)
        {
            var targetFolders = folders.Where(p => p.parent_id == parentId).ToList();
            var targetProducts = products.Where(p => p.folder_id == parentId).ToList();
            if (targetFolders.Any() || targetProducts.Any())
            {
                foreach (var folder in targetFolders)
                {
                    var child = tree.AddChild(new ImgTwinModel
                    {
                        folder = folder.folder,
                        isFile = false,
                        fullpath = tree.Value.fullpath + "/" + folder.folder
                    });
                    FillNextLevelFolders(folders, products, child, folder.id);
                }

                foreach (var product in targetProducts)
                {
                    var imgName = product.id + "." + product.name + "." +
                                  (product.Image != null ? product.Image.img_type : "");
                    tree.AddChild(new ImgTwinModel
                    {
                        folder = imgName,
                        isFile = true,
                        fullpath = tree.Value.fullpath + "/" + imgName
                    });
                }
            }
        }
    }
}
