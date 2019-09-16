using System.Collections.Generic;
using System.Threading.Tasks;
using SmartRetail.App.DAL.Entities;

namespace SmartRetail.App.DAL.Repository.Interfaces
{
    public interface IProductRepository
    {

        Task<Product> GetByIdUow(int id);
        Task<int> InsertUow(Product product);
        Task<bool> UpdateUow(Product product);


        int AddProduct(Product entity);
        Task<Product> GetByIdAsync(int id);
        void UpdateProduct(Product entity);
        IEnumerable<Product> GetProductsByIds(IEnumerable<int> prodIds);
        Task<IEnumerable<Product>> GetProductsByBusinessAsync(int businessId);
        Task<Product> GetByIdAsync(int id, int businessId);
    }
}
