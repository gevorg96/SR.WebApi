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
    internal sealed class GetUnitOfMeasuresQueryHandler : IRequestHandler<GetUnitOfMeasuresQuery, IReadOnlyCollection<Domain.UnitOfMeasure>>
    {
        private readonly ISrContext _db;

        public GetUnitOfMeasuresQueryHandler(ISrContext db) =>
            _db = db;
        
        public async Task<IReadOnlyCollection<Domain.UnitOfMeasure>> Handle(GetUnitOfMeasuresQuery request, CancellationToken cancellationToken) => 
            await _db.UnitOfMeasures.ToListAsync(cancellationToken).ConfigureAwait(false);
    }
    
}