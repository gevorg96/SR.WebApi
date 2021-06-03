using SmartRetail.App.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartRetail.App.DAL.Repository.Interfaces
{
    public interface IBillsRepository
    {
        Task<int> InsertUow(BillParent entity);
        Task<BillParent> GetByIdsWithSalesUow(int billId, int shopId);

        Task<BillParent> GetByIdAsync(int id);
        Task<IEnumerable<BillParent>> GetBillsWithSales(int shopId, DateTime from, DateTime to);
        Task<IEnumerable<BillParent>> GetBillsWithSalesByProdId(int shopId, int prodId, DateTime from, DateTime to);
        Task<int> AddBillAsync(BillParent bill);
        Task<BillParent> GetBillByNumber(int billNumber, DateTime reportDate); 
    }
}
