using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SR.Application.Persistence;

namespace SR.Application.Shop
{
    [UsedImplicitly]
    internal sealed class GetShopByNameQueryHandler: IRequestHandler<GetShopByNameQuery, Domain.Shop>
    {
        private readonly ISrContext _db;

        public GetShopByNameQueryHandler(ISrContext db) =>
            _db = db;

        public async Task<Domain.Shop> Handle(GetShopByNameQuery request, CancellationToken cancellationToken) =>
            await _db.Shops.FirstOrDefaultAsync(x => x.Name.ToUpper() == request.Name.ToUpper(), cancellationToken).ConfigureAwait(false);
    }
}