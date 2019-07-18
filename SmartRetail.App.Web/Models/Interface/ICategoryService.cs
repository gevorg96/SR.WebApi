using SmartRetail.App.DAL.Entities;
using SmartRetail.App.Web.Models.ViewModel;
using System.Threading.Tasks;

namespace SmartRetail.App.Web.Models.Interface
{
    public interface ICategoryService
    {
        Task<ProductGroupViewModel> GetNexLevelGroup(UserProfile user, string fullpath = null, bool needProducts = true);
        Task<ProductGroupViewModel> Search(UserProfile user, string name, string path = null);
    }
}
