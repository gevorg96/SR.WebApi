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
    internal sealed class BusinessesQueryHandler : IRequestHandler<BusinessesQuery, IReadOnlyCollection<Domain.Business>>
    {
        private readonly ISrContext _db;

        public BusinessesQueryHandler(ISrContext db) =>
            _db = db;

        public async Task<IReadOnlyCollection<Domain.Business>> Handle(BusinessesQuery request, CancellationToken cancellationToken)
        {
            var businesses = _db.Businesses.AsQueryable();

            if (!string.IsNullOrEmpty(request.Name))
                businesses = businesses.Where(x => x.Name.ToUpper() == request.Name.ToUpper());
            
            if (request.IncludeShops)
                businesses = businesses.Include(x => x.Shops);

            return await businesses.ToListAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}