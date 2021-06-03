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
    internal sealed class CreateExpenseItemCommandHandler: IRequestHandler<CreateExpenseItemCommand, Domain.ExpenseItem?>
    {
        private readonly ISrContext _db;

        public CreateExpenseItemCommandHandler(ISrContext db) =>
            _db = db;

        public async Task<Domain.ExpenseItem?> Handle(CreateExpenseItemCommand request, CancellationToken token)
        {
            var expense = await _db.Expenses.FirstOrDefaultAsync(x => x.Id == request.ExpenseId, token)
                .ConfigureAwait(false);
            
            var expenseType = await _db.ExpenseTypes.FirstOrDefaultAsync(x => x.Id == request.ExpenseTypeId, token)
                .ConfigureAwait(false);
            
            expense = Guard.Require(expense, request.ExpenseId, "Затраты не найдены");
            expenseType = Guard.Require(expenseType, request.ExpenseTypeId, "Такой тип затрат отсутствует");

            var expenseItem = new Domain.ExpenseItem
            {
                Amount = request.Amount,
                ExpenseId = expense.Id,
                ExpenseTypeId = expenseType.Id
            };
            
            var entity = await _db.AddAsync(expenseItem, token).ConfigureAwait(false);
            
            await _db.SaveChangesAsync(token).ConfigureAwait(false);

            return entity.Entity as Domain.ExpenseItem;
        }
    }
}