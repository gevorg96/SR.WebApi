using SmartRetail.App.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartRetail.App.DAL.Repository.Interfaces
{
    public interface IExpensesTypeRepository
    {
        IEnumerable<ExpensesType> GetAll();
        Task<IEnumerable<ExpensesType>> GetAllAsync();
    }
}
