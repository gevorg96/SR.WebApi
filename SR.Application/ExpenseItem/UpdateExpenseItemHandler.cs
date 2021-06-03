using System;
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
    internal sealed class UpdateExpenseItemHandler : IRequestHandler<UpdateExpenseItem, Unit>
    {
        private readonly ISrContext _db;

        public UpdateExpenseItemHandler(ISrContext db)
        {
            _db = db;
        }

        public async Task<Unit> Handle(UpdateExpenseItem request, CancellationToken cancellationToken)
        {
            var expenseItem = await _db.ExpenseItems
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
                .ConfigureAwait(false);

            Guard.Require(expenseItem, request.Id, "Затрата не найдена");
            
            Domain.ExpenseType? expenseType = null;
            if (expenseItem.ExpenseTypeId != request.ExpenseTypeId)
                expenseType = await _db.ExpenseTypes
                    .FirstOrDefaultAsync(x => x.Id == request.ExpenseTypeId, cancellationToken).ConfigureAwait(false);

            Guard.Require(expenseType, request.ExpenseTypeId, "Такой тип затраты отсутствует");

            expenseItem.Amount = request.Amount;
            expenseItem.ExpenseType = expenseType!;
            expenseItem.ExpenseTypeId = expenseType!.Id;
            
            _db.ExpenseItems.Update(expenseItem);
            await _db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            
            return Unit.Value;
        }
    }
}