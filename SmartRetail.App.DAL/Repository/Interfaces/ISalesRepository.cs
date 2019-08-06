using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartRetail.App.DAL.Entities;

namespace SmartRetail.App.DAL.Repository
{
    public interface ISalesRepository
    {
        Task<IEnumerable<Sales>> GetSalesByShopAndReportDate(int shopId, DateTime from, DateTime to);
        Task AddSalesAsync(Sales sales);
        Task<IEnumerable<Sales>> GetSalesByProdIdAndBill(int billNumber, int prodId);
        Task<IEnumerable<Sales>> GetSalesByBillId(int id);
    }
}
