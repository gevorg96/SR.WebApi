using System;
using System.Collections.Generic;
using System.Linq;
using SmartRetail.App.DAL.Entities;
using SmartRetail.App.DAL.Repository;
using SmartRetail.App.Web.Models.Interface;
using SmartRetail.App.Web.Models.Validation;
using SmartRetail.App.Web.Models.ViewModel;
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

        public IEnumerable<ExpensesViewModel> GetExpenses(UserProfile user, int? shopId, DateTime from, DateTime to)
        {
            var list = new List<ExpensesViewModel>();
            IEnumerable<Expenses> expenses = null;
            
            var avl = _shopsChecker.CheckAvailability(user, shopId);
            if(!avl.isCorrectShop)
                return new List<ExpensesViewModel>();;
            if (!avl.hasShop && avl.isAdmin)
            {   
                expenses = _expRepo.GetExpenses(user.business_id.Value, null, from, to).ToList();
            }
            else if(!avl.hasShop && !avl.isAdmin)
            {
                return new List<ExpensesViewModel>();
            }
            else if(avl.hasShop)
            {
                expenses = _expRepo.GetExpenses(user.business_id.Value, shopId.Value, from, to).ToList();
            }

            if (expenses == null || !expenses.Any())
                return new List<ExpensesViewModel>();;
            
            var groupExpenses = expenses.GroupBy(p => p.report_date).ToList();
            
            foreach (var group in groupExpenses)
            {

                var expensesViewModel = new ExpensesViewModel {reportDate = @group.Key};

                var dict = new List<ExpensesValueViewModel>();
                foreach (var expense in group)
                {
                    dict.Add(new ExpensesValueViewModel
                    {
                        key = expense.ExpensesType.type,
                        value = expense.value
                    });
                }

                expensesViewModel.expenses = dict;
                expensesViewModel.totalSum = dict.Sum(p => p.value);
                list.Add(expensesViewModel);
            }

            return list;
        }
    }
}