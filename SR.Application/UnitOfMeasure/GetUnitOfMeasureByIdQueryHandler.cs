using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SR.Application.Persistence;

namespace SR.Application.UnitOfMeasure
{

    [UsedImplicitly]
    internal sealed class GetUnitOfMeasureByIdQueryHandler : IRequestHandler<GetUnitOfMeasureByIdQuery, Domain.UnitOfMeasure>
    {
        private readonly ISrContext _db;

        public GetUnitOfMeasureByIdQueryHandler(ISrContext db) =>
            _db = db;

        public async Task<Domain.UnitOfMeasure> Handle(GetUnitOfMeasureByIdQuery request, CancellationToken cancellationToken) =>
            await _db.UnitOfMeasures
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
                .ConfigureAwait(false);
    }
}