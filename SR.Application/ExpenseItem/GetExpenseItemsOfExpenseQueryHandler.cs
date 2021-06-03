using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SR.Application.Persistence;
using SR.Domain;

namespace SR.Application.ExpenseItem
{
    [UsedImplicitly]
    internal sealed class GetExpenseItemsOfExpenseQueryHandler: IRequestHandler<GetExpenseItemsOfExpenseQuery, IReadOnlyCollection<Domain.ExpenseItem>>
    {
        private readonly ISrContext _db;
        
        public GetExpenseItemsOfExpenseQueryHandler(ISrContext db) =>
            _db = db;

        public async Task<IReadOnlyCollection<Domain.ExpenseItem>> Handle(GetExpenseItemsOfExpenseQuery request, CancellationToken cancellationToken)
        {
            var expense = await _db.Expenses
                .FirstOrDefaultAsync(x => x.Id == request.ExpenseId, cancellationToken)
                .ConfigureAwait(false);

            Guard.Require(expense, request.ExpenseId, "Затраты не найдены");

            return await _db.ExpenseItems.AsQueryable().Where(x => x.ExpenseId == request.ExpenseId)
                .ToListAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}