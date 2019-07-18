using SmartRetail.App.DAL.Entities;

namespace SmartRetail.App.DAL.Repository
{
    public interface IImageRepository
    {
        void Add(Images entity);
        Images GetById(int id);
        void UpdateImage(int prodId, string field, string value);
    }
}