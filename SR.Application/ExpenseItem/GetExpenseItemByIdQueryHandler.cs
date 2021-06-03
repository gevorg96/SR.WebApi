using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SR.Application.Persistence;

namespace SR.Application.ExpenseItem
{
    [UsedImplicitly]
    internal sealed class GetExpenseItemByIdQueryHandler: IRequestHandler<GetExpenseItemByIdQuery, Domain.ExpenseItem>
    {
        private readonly ISrContext _db;

        public GetExpenseItemByIdQueryHandler(ISrContext db) =>
            _db = db;
        
        public async Task<Domain.ExpenseItem> Handle(GetExpenseItemByIdQuery request, CancellationToken cancellationToken) =>
            await _db.ExpenseItems
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
                .ConfigureAwait(false);
    }
}