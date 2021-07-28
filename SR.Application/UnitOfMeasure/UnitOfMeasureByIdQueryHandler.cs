using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SR.Application.Persistence;

namespace SR.Application.UnitOfMeasure
{

    [UsedImplicitly]
    internal sealed class UnitOfMeasureByIdQueryHandler : IRequestHandler<UnitOfMeasureByIdQuery, Domain.UnitOfMeasure>
    {
        private readonly ISrContext _db;

        public UnitOfMeasureByIdQueryHandler(ISrContext db) => _db = db;

        public async Task<Domain.UnitOfMeasure> Handle(UnitOfMeasureByIdQuery request, CancellationToken cancellationToken) =>
            await _db.UnitOfMeasures
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
                .ConfigureAwait(false);
    }
}