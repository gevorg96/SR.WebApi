using System.Threading.Tasks;
using SmartRetail.App.DAL.Entities;

namespace SmartRetail.App.DAL.Repository.Interfaces
{
    public interface IOrderDetailRepository
    {
        Task<int> InsertUow(OrderDetail orderDetail);
    }
}
