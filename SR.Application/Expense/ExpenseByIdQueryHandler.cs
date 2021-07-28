using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SR.Application.Persistence;

namespace SR.Application.Expense
{
    [UsedImplicitly]
    internal sealed class ExpenseByIdQueryHandler: IRequestHandler<ExpenseByIdQuery, Domain.Expense>
    {
        private readonly ISrContext _db;

        public ExpenseByIdQueryHandler(ISrContext db) =>
            _db = db;

        public async Task<Domain.Expense> Handle(ExpenseByIdQuery request, CancellationToken cancellationToken)
        {
            var expenses = _db.Expenses.AsQueryable();

            if (request.WithExpenseItems.HasValue && request.WithExpenseItems.Value)
                expenses = expenses.Include(x => x.ExpenseItems)
                    .ThenInclude(x => x.ExpenseType);

            return await expenses.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken).ConfigureAwait(false);
        }
    }
}