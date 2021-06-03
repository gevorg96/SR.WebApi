using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SR.Application.Persistence;

namespace SR.Application.Shop
{
    [UsedImplicitly]
    internal sealed class GetShopByIdQueryHandler: IRequestHandler<GetShopByIdQuery, Domain.Shop>
    {
        private readonly ISrContext _db;

        public GetShopByIdQueryHandler(ISrContext db) =>
            _db = db;

        public async Task<Domain.Shop> Handle(GetShopByIdQuery request, CancellationToken cancellationToken) =>
            await _db.Shops.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken).ConfigureAwait(false);
    }
}