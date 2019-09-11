using Newtonsoft.Json.Linq;
using SmartRetail.App.Web.Models.Interface;
using SmartRetail.App.Web.Models.ViewModel.Summary;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using SmartRetail.App.DAL.BLL.DataServices;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Repository.Interfaces;

namespace SmartRetail.App.Web.Models.Service
{
    public class SummaryService : IInformationService
    {
        private readonly ISalesDataService salesDataService;
        private readonly IExpensesDataService expensesDataService;
        private readonly IStocksDataService stocksDataService;
        private readonly IImageRepository imgRepo;
        private readonly IExpensesTypeRepository expTypeRepo;
        private readonly IShopRepository shopRepo;

        public SummaryService(ISalesDataService salesDs, IExpensesDataService expDs, IStocksDataService stocksDs, 
            IImageRepository _imgRepo, IExpensesTypeRepository expensesTypeRepository, IShopRepository shopRepository)
        {
            salesDataService = salesDs;
            imgRepo = _imgRepo;
            expensesDataService = expDs;
            stocksDataService = stocksDs;
            expTypeRepo = expensesTypeRepository;
            shopRepo = shopRepository;
        }

        #region TotalInfo

        public async Task<JObject> GetInfo(int whouse, UserProfile user)
        {
            var summaryVm = new SummaryViewModel();

            if (whouse == 0)
            {
                var shops = shopRepo.GetShopsByBusiness(user.business_id.Value);
            }

            var dailyData = await salesDataService.GeTotalInfoAsync(whouse);
            var shares = await salesDataService.GetSharesAsync(whouse);
            var pair = await salesDataService.GetTop2ProductsAsync(whouse);

            if (pair.Item1 != 0)
            {
                var firstImg = await imgRepo.GetByIdAsync(pair.Item1);
                summaryVm.imgs.Add(firstImg != null && !string.IsNullOrEmpty(firstImg.img_url_temp) ? firstImg.img_url_temp : "Нет продаж");
            }
            if (pair.Item2 != 0)
            {
                var secondImg = await imgRepo.GetByIdAsync(pair.Item1);
                summaryVm.imgs.Add(secondImg != null && !string.IsNullOrEmpty(secondImg.img_url_temp) ? secondImg.img_url_temp : "Нет продаж");
            }

            summaryVm.revenue = dailyData.totalRevenue;
            summaryVm.profit = dailyData.totalProfit;
            summaryVm.salesCount = dailyData.billsCount;
            summaryVm.averageBill = decimal.Round(dailyData.averageBill, 2);

            var dict = new Dictionary<string, decimal>();
            foreach (var s in shares)
            {
                dict.Add(s.category, decimal.Round(s.part, 4));
            }

            var json = JObject.FromObject(summaryVm);
            json.Add(new JProperty("goods", GetInfo(dict)));
            return json;
        }

        #endregion

        public async Task<JObject> GetDailyData(int whouse, UserProfile user)
        {
            var dt = DateTime.Now;
            salesDataService.From = new DateTime(dt.Year, dt.Month, dt.Day);
            return await GetInfo(whouse, user);
        }

        public async Task<JObject> GetExpensesAsync(int whouse, UserProfile user)
        {
            var expenses = await expensesDataService.GetMonthExpensesAsync(whouse, user);
            if (!EnumerableExtensions.Any(expenses))
            {
                var expTypes = await expTypeRepo.GetAllAsync();
                var dict = new Dictionary<string, decimal>();
                foreach (var et in expTypes)
                {   
                    dict.Add(et.type, 0);
                }

                return GetObject(dict);
            }
            return GetObject(expenses);
        }

        public async Task<JObject> GetMonthData(int whouse, UserProfile user)
        {
            var dt = DateTime.Now;
            salesDataService.From = new DateTime(dt.Year, dt.Month, 1);
            return await GetInfo(whouse, user);
        }


        public async Task<JObject> GetStocksAsync(int whouse, UserProfile user)
        {
            var json = new JObject();
            var stocks = await stocksDataService.GetStocks(whouse);
            json.Add(new JProperty("cost", stocks.cost));
            json.Add(new JProperty("goodsCount", stocks.goodsCount));
            json.Add(new JProperty("goods", GetInfo(stocks.goods)));
            return json;
        }



        private static JArray GetInfo(Dictionary<string, decimal> dict)
        {
            var jarray = new JArray();
            foreach (var t in dict)
            {
                jarray.Add(new JObject(new JProperty(t.Key, t.Value)));
            }

            return jarray;
        }

        private static JObject GetObject(Dictionary<string, decimal> dict)
        {
            var json = new JObject();
            foreach (var de in dict)
            {
                json.Add(new JProperty(de.Key, de.Value));
            }

            return json;
        }
    }
}
