using System.Collections.Generic;
using SmartRetail.App.DAL.BLL.HelperClasses;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Helpers;

namespace SmartRetail.App.DAL.BLL.StructureFillers
{
    public interface ICategoryTreeFiller
    {
        Tree<ImgTwinModel> CreateTree(IEnumerable<Folder> folders, IEnumerable<Product> products);
        IEnumerable<ImgTwinModel> GetLevel(string fullPath);
        Tree<ImgTwinModel> SearchSubTree(string fullPath);
        Tree<ImgTwinModel> CreateTreeFolders(IEnumerable<Folder> folders);
    }
}
