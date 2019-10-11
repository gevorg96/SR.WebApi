using System.Threading.Tasks;
using SmartRetail.App.DAL.Entities;

namespace SmartRetail.App.DAL.BLL.DataServices
{
    public interface IOrderDataService
    {
        Task<int> Insert(Order order);
    }
}