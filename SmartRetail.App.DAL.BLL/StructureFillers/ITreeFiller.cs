using SmartRetail.App.DAL.BLL.DataStructures;
using SmartRetail.App.DAL.BLL.HelperClasses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartRetail.App.DAL.BLL.StructureFillers
{
    public interface ITreeFiller
    {
        Task<CathegoryTree<ImgTwinModel>> FillTreeByBusinessAsync(int businessId);
        Task<CathegoryTree<ImgTwinModel>> FillFolderTreeByBusinessAsync(int businessId);
        void AddPath(string path);
        IEnumerable<ImgTwinModel> GetLevel(string fullpath);
        IEnumerable<ImgTwinModel> Search(string search, CathegoryTree<ImgTwinModel> treePart);
        CathegoryTree<ImgTwinModel> SearchSubTree(string fullpath);
        CathegoryTree<ImgTwinModel> Tree { get; }
    }
}
