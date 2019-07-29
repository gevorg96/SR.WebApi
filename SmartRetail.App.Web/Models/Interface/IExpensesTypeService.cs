using SmartRetail.App.Web.Models.ViewModel.ExpensesType;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartRetail.App.Web.Models.Interface
{
    public interface IExpensesTypeService
    {
        Task<IEnumerable<ExpensesTypeViewModel>> GetExpensesTypes();
    }
}
