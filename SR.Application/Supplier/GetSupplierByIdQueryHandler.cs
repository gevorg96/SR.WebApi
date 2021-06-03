using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SR.Application.Persistence;

namespace SR.Application.Supplier
{
    [UsedImplicitly]
    internal sealed class GetSupplierByIdQueryHandler: IRequestHandler<GetSupplierByIdQuery, Domain.Supplier>
    {
        private readonly ISrContext _db;

        public GetSupplierByIdQueryHandler(ISrContext db) =>
            _db = db;

        public async Task<Domain.Supplier> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken) =>
            await _db.Suppliers.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken).ConfigureAwait(false);
    }
}