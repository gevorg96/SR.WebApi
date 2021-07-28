using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SR.Application.Persistence;

namespace SR.Application.Shop
{
    [UsedImplicitly]
    internal sealed class ShopByNameQueryHandler: IRequestHandler<ShopByNameQuery, Domain.Shop>
    {
        private readonly ISrContext _db;

        public ShopByNameQueryHandler(ISrContext db) => _db = db;

        public async Task<Domain.Shop> Handle(ShopByNameQuery request, CancellationToken cancellationToken) =>
            await _db.Shops.FirstOrDefaultAsync(x => x.Name.ToUpper() == request.Name.ToUpper(), cancellationToken).ConfigureAwait(false);
    }
}