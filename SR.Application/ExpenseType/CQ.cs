using System.Collections.Generic;
using MediatR;

namespace SR.Application.ExpenseType
{
    public record GetExpenseTypesQuery : IRequest<IReadOnlyCollection<Domain.ExpenseType>>;

    public record GetExpenseTypeByIdQuery(long Id) : IRequest<Domain.ExpenseType>;

    public record GetExpenseTypeByNameQuery(string Name) : IRequest<Domain.ExpenseType>;

    public record CreateExpenseTypeCommand(string Name) : IRequest<Domain.ExpenseType>;
}