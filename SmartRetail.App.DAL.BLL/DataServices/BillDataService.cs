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
    public class BillDataService: IBillDataService
    {
        private IStrategy _strategy;
        private IBillsRepository _billsRepository;

        public async Task<int> Insert(Bill bill)
        {
            using (var session = new Session())
            {
                var uow = session.UnitOfWork;

                _strategy = new FifoStrategy();
                _billsRepository = new BillsRepository(uow);

                uow.Begin();
                try
                {
                    var billId = await _billsRepository.InsertUow(bill);
                    await _strategy.UpdateAverageCostUow(Direction.Sale, bill, uow);
                    uow.Commit();
                    return billId;
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
