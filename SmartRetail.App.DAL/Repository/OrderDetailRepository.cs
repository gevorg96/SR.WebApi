using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Repository.Interfaces;
using SmartRetail.App.DAL.UnitOfWork;

namespace SmartRetail.App.DAL.Repository
{
    public class OrderDetailRepository: IOrderDetailRepository
    {
        private IUnitOfWork _unitOfWork;

        public OrderDetailRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<int> InsertUow(OrderDetail orderDetail)
        {
            return await _unitOfWork.Connection.InsertAsync(orderDetail, transaction:_unitOfWork.Transaction);
        }
    }
}
