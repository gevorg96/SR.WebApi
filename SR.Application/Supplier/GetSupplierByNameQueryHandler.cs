using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SR.Application.Persistence;

namespace SR.Application.Supplier
{
    [UsedImplicitly]
    internal sealed class GetSupplierByNameQueryHandler: IRequestHandler<GetSupplierByNameQuery, Domain.Supplier>
    {
        private readonly ISrContext _db;

        public GetSupplierByNameQueryHandler(ISrContext db) =>
            _db = db;

        public async Task<Domain.Supplier> Handle(GetSupplierByNameQuery request, CancellationToken cancellationToken) =>
            await _db.Suppliers.FirstOrDefaultAsync(x => x.Name.ToUpper() == request.Name.ToUpper(), cancellationToken).ConfigureAwait(false);
    }
}