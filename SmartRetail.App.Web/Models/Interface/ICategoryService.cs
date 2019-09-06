using SmartRetail.App.DAL.BLL.DataStructures;
using SmartRetail.App.DAL.BLL.HelperClasses;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.Web.Models.ViewModel;
using System.Threading.Tasks;
using SmartRetail.App.DAL.Helpers;

namespace SmartRetail.App.Web.Models.Interface
{
    public interface ICategoryService
    {
        Task<ProductGroupViewModel> GetNexLevelGroup(UserProfile user, string fullpath = null, bool needProducts = true);
        Task<ProductGroupViewModel> Search(UserProfile user, string name, string path = null);
        Task<Tree<ImgTwinModel>> GetFullFolderTree(UserProfile user);
    }
}
