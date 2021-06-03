using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SR.Application.Persistence;

namespace SR.Application.Business
{
    [UsedImplicitly]
    internal sealed class GetBusinessesQueryHandler : IRequestHandler<GetBusinessesQuery, IReadOnlyCollection<Domain.Business>>
    {
        private readonly ISrContext _db;

        public GetBusinessesQueryHandler(ISrContext db) =>
            _db = db;

        public async Task<IReadOnlyCollection<Domain.Business>> Handle(GetBusinessesQuery request, CancellationToken cancellationToken)
        {
            var businesses = _db.Businesses.AsQueryable();

            if (request.IncludeShops)
                businesses = businesses.Include(x => x.Shops);
            
            return await businesses.ToListAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}