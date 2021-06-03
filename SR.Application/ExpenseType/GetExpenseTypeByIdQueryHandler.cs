using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SR.Application.Persistence;

namespace SR.Application.ExpenseType
{
    [UsedImplicitly]
    internal sealed class GetExpenseTypeByIdQueryHandler : IRequestHandler<GetExpenseTypeByIdQuery, Domain.ExpenseType>
    {
        private readonly ISrContext _db;

        public GetExpenseTypeByIdQueryHandler(ISrContext db) =>
            _db = db;

        public async Task<Domain.ExpenseType> Handle(GetExpenseTypeByIdQuery request, CancellationToken cancellationToken) =>
            await _db.ExpenseTypes
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
                .ConfigureAwait(false);
    }
}