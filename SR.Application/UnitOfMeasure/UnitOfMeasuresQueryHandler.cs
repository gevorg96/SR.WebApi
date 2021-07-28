using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SR.Application.Persistence;

namespace SR.Application.UnitOfMeasure
{

    [UsedImplicitly]
    internal sealed class UnitOfMeasuresQueryHandler : IRequestHandler<UnitOfMeasuresQuery, IReadOnlyCollection<Domain.UnitOfMeasure>>
    {
        private readonly ISrContext _db;

        public UnitOfMeasuresQueryHandler(ISrContext db) => _db = db;
        
        public async Task<IReadOnlyCollection<Domain.UnitOfMeasure>> Handle(UnitOfMeasuresQuery request, CancellationToken cancellationToken) => 
            await _db.UnitOfMeasures.ToListAsync(cancellationToken).ConfigureAwait(false);
    }
    
}