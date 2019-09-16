using System.Threading.Tasks;
using SmartRetail.App.DAL.Entities;

namespace SmartRetail.App.DAL.Repository.Interfaces
{
    public interface IImageRepository
    {
        Task<int> InsertUow(Image img);

        void Add(Image entity);
        Task<Image> GetByIdAsync(int id);
        Task UpdateImage(Image img);
    }
}