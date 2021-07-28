using System.Collections.Generic;
using MediatR;

namespace SR.Application.ExpenseType
{
    public record ExpenseTypesQuery : IRequest<IReadOnlyCollection<Domain.ExpenseType>>;

    public record ExpenseTypeByIdQuery(long Id) : IRequest<Domain.ExpenseType>;

    public record ExpenseTypeByNameQuery(string Name) : IRequest<Domain.ExpenseType>;

    public record CreateExpenseTypeCommand(string Name) : IRequest<Domain.ExpenseType>;
}