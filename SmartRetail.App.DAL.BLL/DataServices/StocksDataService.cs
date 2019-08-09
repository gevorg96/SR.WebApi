using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dropbox.Api.TeamLog;
using SmartRetail.App.DAL.BLL.HelperClasses;
using SmartRetail.App.DAL.Repository;
using SmartRetail.App.DAL.Repository.Interfaces;

namespace SmartRetail.App.DAL.BLL.DataServices
{
    public class StocksDataService: IStocksDataService
    {
        private readonly ICostRepository costRepo;
        private readonly IStockRepository stockRepo;
        private readonly IImageRepository imgRepo;

        public StocksDataService(ICostRepository costRepository, IStockRepository stockRepository, IImageRepository imageRepository)
        {
            costRepo = costRepository;
            stockRepo = stockRepository;
            imgRepo = imageRepository;
        }

        public async Task<SummaryStocks> GetStocks(int shopId)
        {
            var stocksModel = new SummaryStocks();

            var stocksDal = await stockRepo.GetStocksWithProducts(shopId);
            if (stocksDal == null || !stocksDal.Any())
            {
                return stocksModel;
            }

            decimal goodsCount = 0;
            decimal goodsCost = 0;
            var dict = new Dictionary<string, decimal>();
            foreach (var stock in stocksDal)
            {
                goodsCount += stock.count ?? 0;
                var cost = costRepo.GetByProdId(stock.prod_id).FirstOrDefault();
                goodsCost += cost?.value != null ? cost.value.Value * (stock.count ?? 0) : 0;

                var img = await imgRepo.GetByIdAsync(stock.prod_id);
                var cat = img != null && !string.IsNullOrEmpty(img.img_path) ? img.img_path.Split('/')[3] : null;
                if (cat != null)
                {
                    if (dict.ContainsKey(cat))
                    {
                        dict[cat] += stock.count ?? 0;
                    }
                    else
                    {
                        dict.Add(cat, stock.count ?? 0);
                    }
                }
                else
                {
                    var withoutCat = "Без категории";
                    if (dict.ContainsKey(withoutCat))
                    {
                        dict[withoutCat] += stock.count ?? 0;
                    }
                    else
                    {
                        dict.Add(withoutCat, stock.count ?? 0);
                    }
                }
            }

            stocksModel.cost = goodsCost;
            stocksModel.goodsCount = goodsCount;
            stocksModel.goods = dict;

            return stocksModel;
        }
    }
}
