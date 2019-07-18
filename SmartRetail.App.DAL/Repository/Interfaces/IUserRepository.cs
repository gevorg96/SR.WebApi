using SmartRetail.App.DAL.Entities;

namespace SmartRetail.App.DAL.Repository
{
    public interface IUserRepository
    {
        UserProfile GetByLogin(string login);
        UserProfile GetById(int id);
    }
}
