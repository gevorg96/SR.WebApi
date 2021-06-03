using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SR.Application.Persistence;

namespace SR.Application.Product
{
    [UsedImplicitly]
    internal sealed class GetProductByIdQueryHandler: IRequestHandler<GetProductByIdQuery, Domain.Product>
    {
        private readonly ISrContext _db;

        public GetProductByIdQueryHandler(ISrContext db) =>
            _db = db;

        public async Task<Domain.Product> Handle(GetProductByIdQuery request, CancellationToken cancellationToken) => 
            await _db.Products
                .Include(x => x.ProductProperties)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
                .ConfigureAwait(false);
    }
}