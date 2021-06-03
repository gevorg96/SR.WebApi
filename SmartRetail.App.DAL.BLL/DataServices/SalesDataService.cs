using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartRetail.App.DAL.BLL.HelperClasses;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Repository.Interfaces;

namespace SmartRetail.App.DAL.BLL.DataServices
{
    public class SalesDataService : ISalesDataService
    {
        private readonly IImageRepository _imagesRepo;
        private readonly IBillsRepository _billRepo;
        private readonly IBusinessRepository _businessRepo;
        public DateTime From { get; set; }

        private IEnumerable<BillParent> bills;

        public SalesDataService(IProductRepository pRepo, IImageRepository iRepo,
            IBillsRepository bRepo, IBusinessRepository businessRepo)
        {
            _imagesRepo = iRepo;
            _billRepo = bRepo;
            _businessRepo = businessRepo;
        }
        
        public async Task<DailyInfo> GeTotalInfoAsync(int shopId)
        {
            var info = new DailyInfo();
            var dt = DateTime.Now;
            
            bills = await _billRepo.GetBillsWithSales(shopId, From, dt);
            if (bills == null || !bills.Any())
            {
                return new DailyInfo
                {
                    averageBill = 0,
                    billsCount = 0,
                    totalProfit = 0,
                    totalRevenue = 0
                };
            }

            var sales = new List<Sale>();
            foreach (var bill in bills)
            {
                sales.AddRange(bill.Sales);
            }

            info.averageBill = bills.Average(p => p.sum);
            info.billsCount = bills.Count();
            info.totalRevenue = sales.Sum(p => p.sum);
            info.totalProfit = sales.Sum(p => p.profit);

            return info;
        }

        public async Task<IEnumerable<SalesShares>> GetSharesAsync(int shopId)
        {
            if (bills == null || !bills.Any())
            {
                return new List<SalesShares> {new SalesShares {category = "Нет продаж", part = 1}};
            }

            var sales = new List<Sale>();
            foreach (var bill in bills)
            {
                sales.AddRange(bill.Sales);
            }

            var dict = new Dictionary<string, decimal>();
            foreach (var sale in sales)
            {
                var img = await _imagesRepo.GetByIdAsync(sale.prod_id);
                var cat = img != null && !string.IsNullOrEmpty(img.img_path) ? img.img_path.Split('/')[3] : null;
                if (cat != null)
                {
                    if (dict.ContainsKey(cat))
                    {
                        dict[cat] += sale.sum;
                    }
                    else
                    {
                        dict.Add(cat, sale.sum);
                    }
                }
                else
                {
                    var withoutCat = "Без категории";
                    if (dict.ContainsKey(withoutCat))
                    {
                        dict[withoutCat] += sale.sum;
                    }
                    else
                    {
                        dict.Add(withoutCat, sale.sum);
                    }
                }
            }

            var all = dict.Sum(p => p.Value);

            return dict.Select(p => new SalesShares {category = p.Key, part = p.Value / all});
        }

        public (int, int) GetTop2ProductsAsync(int shopId)
        {
            if (bills == null || !bills.Any())
            {
                return (0, 0);
            }
            var sales = new List<Sale>();
            foreach (var bill in bills)
            {
                sales.AddRange(bill.Sales);
            }

            if (sales.Any())
            {
                var groups = sales.GroupBy(p => p.prod_id);
                var dict = groups.ToDictionary(@group => @group.Key, @group => @group.Sum(p => p.sum));
                if (dict.Any())
                {
                    var orderedDict = dict.OrderBy(p => p.Value);
                    if (orderedDict.Any())
                    {
                        var pair = orderedDict.Skip(orderedDict.Count() - 2).Take(2);
                        return (pair.First().Key, pair.Last().Key);
                    }
                    
                }
            }

            return (0, 0);
        }
    }
}
