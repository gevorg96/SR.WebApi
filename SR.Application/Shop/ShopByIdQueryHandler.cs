using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SR.Application.Persistence;

namespace SR.Application.Shop
{
    [UsedImplicitly]
    internal sealed class ShopByIdQueryHandler: IRequestHandler<ShopByIdQuery, Domain.Shop>
    {
        private readonly ISrContext _db;

        public ShopByIdQueryHandler(ISrContext db) => _db = db;

        public async Task<Domain.Shop> Handle(ShopByIdQuery request, CancellationToken cancellationToken) =>
            await _db.Shops.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken).ConfigureAwait(false);
    }
}