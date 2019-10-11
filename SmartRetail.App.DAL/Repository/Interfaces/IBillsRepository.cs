using SmartRetail.App.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartRetail.App.DAL.Repository.Interfaces
{
    public interface IBillsRepository
    {
        Task<int> InsertUow(Bill entity);
        Task<Bill> GetByIdsWithSalesUow(int billId, int shopId);

        Task<Bill> GetByIdAsync(int id);
        Task<IEnumerable<Bill>> GetBillsWithSales(int shopId, DateTime from, DateTime to);
        Task<IEnumerable<Bill>> GetBillsWithSalesByProdId(int shopId, int prodId, DateTime from, DateTime to);
        Task<int> AddBillAsync(Bill bill);
        Task<Bill> GetBillByNumber(int billNumber, DateTime reportDate); 
    }
}
