using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SR.Application.Persistence;

namespace SR.Application.Shop
{
    [UsedImplicitly]
    internal sealed class ShopsQueryHandler: IRequestHandler<ShopsQuery, IReadOnlyCollection<Domain.Shop>>
    {
        private readonly ISrContext _db;

        public ShopsQueryHandler(ISrContext db) => _db = db;

        public async Task<IReadOnlyCollection<Domain.Shop>> Handle(ShopsQuery request, CancellationToken cancellationToken)
        {
            var shops = _db.Shops.AsQueryable();
            if (request.BusinessId > 0)
                shops = shops.Where(x => x.BusinessId == request.BusinessId);
            
            return await shops.ToListAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}