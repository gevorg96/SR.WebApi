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
    internal sealed class ExpenseTypeQueryHandler : IRequestHandler<ExpenseTypesQuery, IReadOnlyCollection<Domain.ExpenseType>>
    {
        private readonly ISrContext _db;

        public ExpenseTypeQueryHandler(ISrContext db) =>
            _db = db;
        
        public async Task<IReadOnlyCollection<Domain.ExpenseType>> Handle(ExpenseTypesQuery request, CancellationToken cancellationToken) => 
            await _db.ExpenseTypes.ToListAsync(cancellationToken).ConfigureAwait(false);
    }
    
}