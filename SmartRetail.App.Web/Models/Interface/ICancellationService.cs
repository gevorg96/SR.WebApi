using SmartRetail.App.DAL.Entities;
using SmartRetail.App.Web.Models.ViewModel.Orders;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartRetail.App.Web.Models.Interface
{
    public interface ICancellationService
    {
        Task<IEnumerable<OrderViewModel>> GetCancellations(UserProfile user, DateTime from, DateTime to, int shopId);
    }
}
