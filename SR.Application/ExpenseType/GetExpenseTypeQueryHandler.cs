using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SR.Application.Persistence;

namespace SR.Application.ExpenseType
{
    [UsedImplicitly]
    internal sealed class GetExpenseTypeQueryHandler : IRequestHandler<GetExpenseTypesQuery, IReadOnlyCollection<Domain.ExpenseType>>
    {
        private readonly ISrContext _db;

        public GetExpenseTypeQueryHandler(ISrContext db) =>
            _db = db;
        
        public async Task<IReadOnlyCollection<Domain.ExpenseType>> Handle(GetExpenseTypesQuery request, CancellationToken cancellationToken) => 
            await _db.ExpenseTypes.ToListAsync(cancellationToken).ConfigureAwait(false);
    }
    
}