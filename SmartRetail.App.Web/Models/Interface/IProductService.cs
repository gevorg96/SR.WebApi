using System.Collections.Generic;
using System.Threading.Tasks;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.Web.Models.ViewModel;
using SmartRetail.App.Web.Models.ViewModel.Products;

namespace SmartRetail.App.Web.Models.Interface
{
    public interface IProductService
    {
        Task<IEnumerable<ProductViewModel>> GetProducts(UserProfile user);
        Task<ProductDetailViewModel> AddProduct(UserProfile user, ProductDetailViewModel product);
        Task<ProductViewModel> UpdateProduct(UserProfile user, ProductDetailViewModel product);
        Task<ProductViewModel> GetProduct(UserProfile user, int id);

        Task<ProductDetailViewModel> AddProductTransaction(UserProfile user, ProductDetailViewModel product);
        Task<ProductViewModel> UpdateProductTransaction(UserProfile user, ProductDetailViewModel product);
    }
}
