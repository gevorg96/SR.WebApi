using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartRetail.App.Web.Models.ViewModel;
using SmartRetail.App.Web.Models.ViewModel.Sales;

namespace SmartRetail.App.Web.Models.Interface
{
    public interface ISalesService
    {
        Task<IEnumerable<SalesViewModel>> GetSales(int userId, int shopId, DateTime from, DateTime to);
        Task AddSale(SalesCreateViewModel model);
    }
}
