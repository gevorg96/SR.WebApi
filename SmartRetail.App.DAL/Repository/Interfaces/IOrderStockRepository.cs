﻿using SmartRetail.App.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartRetail.App.DAL.Repository.Interfaces
{
    public interface IOrderStockRepository
    {
        Task<IEnumerable<OrderStock>> GetOrderStocksByProdIds(IEnumerable<int> prodIds);
        Task<IEnumerable<OrderStock>> GetOrderStocksByProdId(int prodId);
    }
}