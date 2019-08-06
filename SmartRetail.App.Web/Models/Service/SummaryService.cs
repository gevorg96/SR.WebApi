using Newtonsoft.Json.Linq;
using SmartRetail.App.DAL.Repository;
using SmartRetail.App.Web.Models.Interface;
using SmartRetail.App.Web.Models.ViewModel.Summary;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SmartRetail.App.DAL.BLL.DataServices;

namespace SmartRetail.App.Web.Models.Service
{
    public class SummaryService : IInformationService
    {
        private readonly ISalesDataService salesDataService;
        private readonly IImageRepository imgRepo;
        public SummaryService(ISalesDataService salesDs, IImageRepository _imgRepo)
        {
            salesDataService = salesDs;
            imgRepo = _imgRepo;
        }

        #region TotalInfo

        public async Task<JObject> GetInfo(int whouse)
        {
            var summaryVm = new SummaryViewModel();

            var dailyData = await salesDataService.GetDailyInfo(whouse);
            var shares = await salesDataService.GetShares(whouse);
            var pair = await salesDataService.GetTop2Products(whouse);

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

        public async Task<JObject> GetDailyData(int whouse)
        {
            var dt = DateTime.Now;
            salesDataService.From = new DateTime(dt.Year, dt.Month, dt.Day);
            return await GetInfo(whouse);
        }

        public JObject GetExpenses(int whouse)
        {
            throw new NotImplementedException();
        }

        public async Task<JObject> GetMonthData(int whouse)
        {
            var dt = DateTime.Now;
            salesDataService.From = new DateTime(dt.Year, dt.Month, 1);
            return await GetInfo(whouse);
        }

        public JObject GetStocks(int whouse)
        {
            throw new NotImplementedException();
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

    }
}
