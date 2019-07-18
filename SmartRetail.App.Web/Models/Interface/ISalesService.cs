using System;
using System.Collections.Generic;
using SmartRetail.App.Web.Models.ViewModel;

namespace SmartRetail.App.Web.Models.Interface
{
    public interface ISalesService
    {
        IEnumerable<SalesViewModel> GetSales(int userId, int shopId, DateTime from, DateTime to);
    }
}
