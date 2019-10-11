using System;
using System.Threading.Tasks;
using SmartRetail.App.DAL.BLL.Utils;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Helpers;
using SmartRetail.App.DAL.Repository;
using SmartRetail.App.DAL.Repository.Interfaces;
using SmartRetail.App.DAL.UnitOfWork;

namespace SmartRetail.App.DAL.BLL.DataServices
{
    public class OrderDataService: IOrderDataService
    {
        private IStrategy _strategy;
        private IOrdersRepository _ordersRepository;
        
        public async Task<int> Insert(Order order)
        {
            using (var session = new Session())
            {
                var uow = session.UnitOfWork;

                _strategy = new FifoStrategy();
                _ordersRepository = new OrdersRepository(uow);

                uow.Begin();
                try
                {
                    var orderId = await _ordersRepository.InsertUow(order);
                    if (orderId == 0)
                    {
                        uow.RollBack();
                        throw new Exception("Отсутствует приход.");
                    }

                    order.id = orderId;
                    await _strategy.UpdateAverageCostUow( order.isOrder ? Direction.Order: Direction.Cancellation, order, uow);
                    uow.Commit();
                    return orderId;
                }
                catch (Exception e)
                {
                    uow.RollBack();
                    throw e;
                }
            }
        }
    }
}