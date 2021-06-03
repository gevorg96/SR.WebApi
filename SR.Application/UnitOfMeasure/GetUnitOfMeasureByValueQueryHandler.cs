using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SR.Application.Persistence;

namespace SR.Application.UnitOfMeasure
{

    [UsedImplicitly]
    internal sealed class GetUnitOfMeasureByValueQueryHandler : IRequestHandler<GetUnitOfMeasureByNameQuery, Domain.UnitOfMeasure>
    {
        private readonly ISrContext _db;

        public GetUnitOfMeasureByValueQueryHandler(ISrContext db) =>
            _db = db;

        public async Task<Domain.UnitOfMeasure> Handle(GetUnitOfMeasureByNameQuery request, CancellationToken cancellationToken) =>
            await _db.UnitOfMeasures
                .FirstOrDefaultAsync(x => x.Name.ToUpper() == request.Name.ToUpper(), cancellationToken)
                .ConfigureAwait(false);
    }
}