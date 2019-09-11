using System.Threading.Tasks;
using SmartRetail.App.DAL.Entities;

namespace SmartRetail.App.DAL.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<UserProfile> GetByLogin(string login);
        UserProfile GetById(int id);
    }
}
