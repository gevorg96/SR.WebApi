﻿using System.Collections.Generic;
using System.Threading.Tasks;
using SmartRetail.App.DAL.Entities;

namespace SmartRetail.App.DAL.Repository.Interfaces
{
    public interface IStockRepository
    {
        Task<IEnumerable<Stock>> GetStocksWithProducts(int shopId);
        IEnumerable<Stock> GetStocksWithProductsByBusiness(int businessId);
        Stock GetStockByShopAndProdIds(int shopId, int prodId);
        Task AddAsync(Stock entity);
        Task UpdateValueAsync(Stock entity);

    }
}
