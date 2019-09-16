using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Repository.Interfaces;
using SmartRetail.App.Web.Models.Interface;
using SmartRetail.App.Web.Models.Validation;
using SmartRetail.App.Web.Models.ViewModel.Expenses;

namespace SmartRetail.App.Web.Models.Service
{
    public class ExpensesService: IExpensesService
    {
        private readonly IExpensesRepository _expRepo;
        private readonly ShopsChecker _shopsChecker;

        public ExpensesService(IExpensesRepository expRepo, ShopsChecker shopsChecker)
        {
            _expRepo = expRepo;
            _shopsChecker = shopsChecker;
        }

        public async Task<IEnumerable<ExpensesViewModel>> GetExpenses(UserProfile user, int? shopId, DateTime from, DateTime to)
        {
            var list = new List<ExpensesViewModel>();
            IEnumerable<Expense> expenses = null;
            
            var avl = _shopsChecker.CheckAvailability(user, shopId);
            if(!avl.isCorrectShop)
                return new List<ExpensesViewModel>();;
            if (!avl.hasShop && avl.isAdmin)
            {   
                expenses = await _expRepo.GetExpensesAsync(user.business_id.Value, null, from, to);
            }
            else if(!avl.hasShop && !avl.isAdmin)
            {
                return new List<ExpensesViewModel>();
            }
            else if(avl.hasShop)
            {
                expenses = await _expRepo.GetExpensesAsync(user.business_id.Value, shopId.Value, from, to);
            }

            if (expenses == null || !expenses.Any())
                return new List<ExpensesViewModel>();;
            
            
            
            foreach (var group in expenses)
            {
                var expensesViewModel = new ExpensesViewModel { id = group.id, reportDate = group.report_date, totalSum = group.sum, shopId = group.shop_id};

                var dict = new List<ExpensesValueViewModel>();
                foreach (var ed in group.ExpensesDetails)
                {
                    dict.Add(new ExpensesValueViewModel
                    {
                        id = ed.expenses_type_id,
                        key = ed.ExpensesType.type,
                        value = ed.sum
                    });
                }

                expensesViewModel.expenses = dict;
                list.Add(expensesViewModel);
            }

            return list;
        }

        public async Task<ExpensesViewModel> AddExpenses(UserProfile user, ExpensesRequestViewModel model)
        {
            var expenses = new Expense
            {
                business_id = user.business_id.Value,
                shop_id = model.shopId,
                sum = model.totalSum,
                report_date = model.reportDate,
                ExpensesDetails = model.expenses.Select(p => new ExpensesDetail
                {
                    expenses_type_id = Convert.ToInt32(p.id), sum = p.value
                }).ToList()
            };


            var exId = 0;
            try
            {
                exId = await _expRepo.AddExpenses(expenses);
                model.id = exId;
            }
            catch (Exception)
            {

                throw new Exception("Добавление расхода не удалось.");
            }

            var expensesDal = await _expRepo.GetByIdAsync(exId);
            var expensesVm = new ExpensesViewModel
            {
                id = expensesDal.id,
                shopId = expensesDal.shop_id,
                reportDate = expensesDal.report_date,
                totalSum = expensesDal.sum,
                expenses = new List<ExpensesValueViewModel>(),
            };
            foreach (var expD in expensesDal.ExpensesDetails)
            {
                expensesVm.expenses.Add(new ExpensesValueViewModel
                {
                    key = expD.ExpensesType.type,
                    value = expD.sum
                });
            }

            return expensesVm;
        }

    }
}