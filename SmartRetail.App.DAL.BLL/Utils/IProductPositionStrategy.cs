using System;
using System.Threading.Tasks;

namespace SmartRetail.App.DAL.BLL.Utils
{
    public interface IProductPositionStrategy
    {
        Task<double> GetProductPositionOffDays(int shopId, int productId, DateTime from, DateTime to);
    }
}