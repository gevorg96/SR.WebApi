using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SR.Application.Persistence;

namespace SR.Application.ExpenseItem
{
    [UsedImplicitly]
    internal sealed class ExpenseItemByIdQueryHandler: IRequestHandler<ExpenseItemByIdQuery, Domain.ExpenseItem>
    {
        private readonly ISrContext _db;

        public ExpenseItemByIdQueryHandler(ISrContext db) => _db = db;
        
        public async Task<Domain.ExpenseItem> Handle(ExpenseItemByIdQuery request, CancellationToken cancellationToken) =>
            await _db.ExpenseItems
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
                .ConfigureAwait(false);
    }
}