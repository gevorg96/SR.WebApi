using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Helpers;
using System.Threading.Tasks;

namespace SmartRetail.App.DAL.BLL.Utils
{
    public interface IStrategy
    {
        Task UpdateAverageCost(Direction direction, IEntity entity, int productId, int shopId);
    }
}
