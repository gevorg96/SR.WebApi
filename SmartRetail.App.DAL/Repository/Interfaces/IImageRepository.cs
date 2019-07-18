using SmartRetail.App.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartRetail.App.DAL.Repository
{
    public interface IImageRepository
    {
        void Add(Images entity);
        Images GetById(int id);
        void UpdateImage(int prodId, string field, string value);
        Task<IEnumerable<Images>> GetAllImagesInBusinessAsync(int businessId);
    }
}