using System;
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
    internal sealed class GetBusinessQueryHandler : IRequestHandler<GetBusinessQuery, Domain.Business>
    {
        private readonly ISrContext _db;

        public GetBusinessQueryHandler(ISrContext db) =>
            _db = db;

        public async Task<Domain.Business> Handle(GetBusinessQuery request, CancellationToken cancellationToken)
        {
            var (id, name, includeShops) = request;

            var hasId = id.HasValue;
            var hasName = !string.IsNullOrEmpty(name);
            
            if (hasId && hasName || !hasId && !hasName)
                throw new ArgumentException("Необходимо указать либо Id, либо Name");
            
            var business = _db.Businesses.AsQueryable();

            if (includeShops.HasValue && includeShops.Value)
                business = business.Include(x => x.Shops);

            if (hasId)
                return await business.FirstOrDefaultAsync(x => x.Id == id, cancellationToken).ConfigureAwait(false);

            return await business.FirstOrDefaultAsync(x => x.Name.ToUpper() == name.ToUpper(), cancellationToken).ConfigureAwait(false);
        }
        
    }
}