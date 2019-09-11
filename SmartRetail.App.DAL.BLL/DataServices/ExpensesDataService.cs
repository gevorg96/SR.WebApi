using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Repository.Interfaces;

namespace SmartRetail.App.DAL.BLL.DataServices
{
    public class ExpensesDataService: IExpensesDataService
    {
        private readonly IExpensesRepository expRepo;
        private readonly IExpensesTypeRepository expTypeRepo;

        public ExpensesDataService(IExpensesRepository _expRepo, IExpensesTypeRepository _expTypeRepo)
        {
            expRepo = _expRepo;
            expTypeRepo = _expTypeRepo;
        }

        public async Task<Dictionary<string, decimal>> GetMonthExpensesAsync(int shopId, UserProfile user)
        {
            var dt = DateTime.Now;
            var resultDict = new Dictionary<string, decimal>();
            var business = user.business_id.Value;
            int? shop = 0;

            if (shopId == 0 && user.shop_id == null)
            {
                shop = null;
            }
            else
            {
                shop = shopId;
            }
            
            var expenses = await expRepo.GetExpensesAsync(business, shop, new DateTime(dt.Year, dt.Month, 1), dt);

            var expTypes = await expTypeRepo.GetAllAsync();

            foreach (var exp in expenses)
            {
                foreach (var expDetail in exp.ExpensesDetails)
                {
                    if (!resultDict.ContainsKey(expDetail.ExpensesType.type))
                    {
                        resultDict.Add(expDetail.ExpensesType.type, expDetail.sum);
                    }
                    else
                    {
                        resultDict[expDetail.ExpensesType.type] += expDetail.sum;
                    }
                }
            }

            return resultDict;
        }
    }
}
