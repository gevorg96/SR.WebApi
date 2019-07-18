using System.Collections.Generic;
using System.Threading.Tasks;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.Web.Models.ViewModel;
using SmartRetail.App.Web.Models.ViewModel.Products;

namespace SmartRetail.App.Web.Models.Interface
{
    public interface IProductService
    {
        IEnumerable<ProductViewModel> GetProducts(UserProfile user);
        Task AddProduct(UserProfile user, ProductDetailViewModel product);
        void ChangePath(UserProfile user, int prodId, string newPath);
        Task<ProductGroupViewModel> GetNexLevelGroup(UserProfile user, string fullpath, bool needProducts=true);
        Task<ProductGroupViewModel> Search(UserProfile user, string name, ulong start, ulong limit, string path = null);
        ProductDetailRequestViewModel GetChoiceForUser(UserProfile user);
        Task UpdateProduct(UserProfile user, ProductDetailViewModel product);
        ProductViewModel GetProduct(UserProfile user, int id);
    }
}
