using SmartRetail.App.DAL.Repository.Interfaces;
using SmartRetail.App.Web.Models.Interface;
using SmartRetail.App.Web.Models.ViewModel.ExpensesType;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartRetail.App.Web.Models.Service
{
    public class ExpensesTypeService : IExpensesTypeService
    {
        private readonly IExpensesTypeRepository repo;
        public ExpensesTypeService(IExpensesTypeRepository repository)
        {
            repo = repository;
        }

        public async Task<IEnumerable<ExpensesTypeViewModel>> GetExpensesTypes()
        {
            var etDal = await repo.GetAllAsync();
            var etList = new List<ExpensesTypeViewModel>();
            foreach (var et in etDal)
            {
                etList.Add(
                    new ExpensesTypeViewModel
                    {
                        id = et.id,
                        value = et.type
                    }
                );
            }

            return etList;
        }
    }
}
