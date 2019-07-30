using SmartRetail.App.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartRetail.App.DAL.Repository
{
    public interface IImageRepository
    {
        void Add(Images entity);
        Task<Images> GetByIdAsync(int id);
        void UpdateImage(int prodId, string field, string value);
        Task UpdateImage(Images img);
        IEnumerable<Images> GetAllImages();
        Task<IEnumerable<Images>> GetAllImagesInBusinessAsync(int businessId);
    }
}