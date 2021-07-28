using System.Collections.Generic;
using MediatR;

namespace SR.Application.ExpenseItem
{
    public record ExpenseItemByIdQuery(long Id) : IRequest<Domain.ExpenseItem>;
    public record ExpenseItemsOfExpenseQuery(long ExpenseId) : IRequest<IReadOnlyCollection<Domain.ExpenseItem>>;
    public record CreateExpenseItemCommand(decimal Amount, long ExpenseId, long ExpenseTypeId) : IRequest<Domain.ExpenseItem?>;

    public record UpdateExpenseItemCommand(long Id, decimal Amount, long ExpenseTypeId) : IRequest<Unit>;
    
    public record DeleteExpenseItemCommand(long Id) : IRequest<Unit>;
}