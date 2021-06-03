using System;
using System.Collections.Generic;
using MediatR;

namespace SR.Application.Expense
{
    public record GetExpenseByIdQuery(long Id, bool? WithExpenseItems) : IRequest<Domain.Expense>;
    
    public record GetExpenseQuery(long BusinessId, long? ShopId, bool? WithExpenseItems, DateTimeOffset? On, DateTimeOffset? From, DateTimeOffset? To) : IRequest<IReadOnlyCollection<Domain.Expense>>;
    
    public record CreateExpenseCommand(DateTimeOffset ReportDate, decimal Amount, long BusinessId, 
        long ShopId, IEnumerable<Domain.ExpenseItem> Items) : IRequest<Domain.Expense?>;
}