using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SR.Application.Persistence;

namespace SR.Application.ExpenseType
{
    [UsedImplicitly]
    internal sealed class GetExpenseTypeByValueQueryHandler : IRequestHandler<GetExpenseTypeByNameQuery, Domain.ExpenseType>
    {
        private readonly ISrContext _db;

        public GetExpenseTypeByValueQueryHandler(ISrContext db) =>
            _db = db;

        public async Task<Domain.ExpenseType> Handle(GetExpenseTypeByNameQuery request, CancellationToken cancellationToken) =>
            await _db.ExpenseTypes
                .FirstOrDefaultAsync(x => x.Name.ToUpper() == request.Name.ToUpper(), cancellationToken)
                .ConfigureAwait(false);
    }
}