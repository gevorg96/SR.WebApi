using System.Threading.Tasks;
using SmartRetail.App.DAL.Entities;

namespace SmartRetail.App.DAL.Repository.Interfaces
{
    public interface IImageRepository
    {
        void Add(Images entity);
        Task<Images> GetByIdAsync(int id);
        Task UpdateImage(Images img);
    }
}