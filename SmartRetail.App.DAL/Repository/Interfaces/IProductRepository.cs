using System.Collections.Generic;
using System.Threading.Tasks;
using SmartRetail.App.DAL.Entities;

namespace SmartRetail.App.DAL.Repository
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetWithFilter(string field, string value);
        IEnumerable<Product> GetAll();
        int AddProduct(Product entity);
        Task<Product> GetByIdAsync(int id);
        void UpdateProduct(Product entity, string field);
        void UpdateProduct(Product entity);
        IEnumerable<Product> GetProductsByIds(IEnumerable<int> prodIds);
        Task<IEnumerable<Product>> GetProductsByBusinessAsync(int businessId);
        Task<Product> GetByIdAsync(int id, int businessId);
    }
}
