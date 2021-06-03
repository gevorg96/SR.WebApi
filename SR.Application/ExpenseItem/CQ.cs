using System.Collections.Generic;
using MediatR;

namespace SR.Application.ExpenseItem
{
    public record GetExpenseItemByIdQuery(long Id) : IRequest<Domain.ExpenseItem>;
    public record GetExpenseItemsOfExpenseQuery(long ExpenseId) : IRequest<IReadOnlyCollection<Domain.ExpenseItem>>;
    public record CreateExpenseItemCommand(decimal Amount, long ExpenseId, long ExpenseTypeId) : IRequest<Domain.ExpenseItem?>;

    public record UpdateExpenseItem(long Id, decimal Amount, long ExpenseTypeId) : IRequest<Unit>;
    
    public record DeleteExpenseItem(long Id) : IRequest<Unit>;
}