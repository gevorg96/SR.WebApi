using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Helpers;
using System.Threading.Tasks;
using SmartRetail.App.DAL.UnitOfWork;

namespace SmartRetail.App.DAL.BLL.Utils
{
    public interface IStrategy
    {
        Task UpdateAverageCost(Direction direction, IEntity entity);
        Task UpdateAverageCostUow(Direction direction, IEntity entity, IUnitOfWork uow);
    }
}
