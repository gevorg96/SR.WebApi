using System;
using SmartRetail.App.DAL.BLL.HelperClasses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartRetail.App.DAL.BLL.DataServices
{
    public interface ISalesDataService
    {
        DateTime From { get; set; }
        Task<DailyInfo> GeTotalInfoAsync(int shopId);
        Task<(int, int)> GetTop2ProductsAsync(int shopId);
        Task<IEnumerable<SalesShares>> GetSharesAsync(int shopId);
    }
}
