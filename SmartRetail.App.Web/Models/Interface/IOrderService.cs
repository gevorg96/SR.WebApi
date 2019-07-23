using SmartRetail.App.DAL.Entities;
using SmartRetail.App.Web.Models.ViewModel.Orders;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartRetail.App.Web.Models.Interface
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderViewModel>> GetOrders(UserProfile user, DateTime from, DateTime to, int shopId);
        Task AddOrder(OrderCreateViewModel model);
    }
}
