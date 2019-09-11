using SmartRetail.App.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartRetail.App.DAL.Repository.Interfaces
{
    public interface IBillsRepository
    {
        Task<Bills> GetByIdAsync(int id);
        Task<IEnumerable<Bills>> GetBillsWithSales(int shopId, DateTime from, DateTime to);
        Task<int> AddBillAsync(Bills bill);
        Task<Bills> GetBillByNumber(int billNumber, DateTime reportDate); 
    }
}
