using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartRetail.App.DAL.Entities;

namespace SmartRetail.App.DAL.Repository
{
    public interface ISalesRepository
    {
        IEnumerable<Sales> GetSalesByShopAndReportDate(int shopId, DateTime from, DateTime to);
        void AddSales(Sales sales);

    }
}
