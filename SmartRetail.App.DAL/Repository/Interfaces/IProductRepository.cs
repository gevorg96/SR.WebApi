using System.Collections.Generic;
using SmartRetail.App.DAL.Entities;

namespace SmartRetail.App.DAL.Repository
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetAllProductsInShop(int shopId);
        int AddProduct(Product entity);
        Product GetById(int id);
        void UpdateProduct(Product entity, string field);
        void UpdateProduct(Product entity);
        IEnumerable<Product> GetProductsByIds(IEnumerable<int> prodIds);
    }
}
