using SmartRetail.App.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartRetail.App.DAL.Repository.Interfaces
{
    public interface IBillsRepository
    {
        Task<Bill> GetByIdAsync(int id);
        Task<IEnumerable<Bill>> GetBillsWithSales(int shopId, DateTime from, DateTime to);
        Task<int> AddBillAsync(Bill bill);
        Task<Bill> GetBillByNumber(int billNumber, DateTime reportDate); 
    }
}
