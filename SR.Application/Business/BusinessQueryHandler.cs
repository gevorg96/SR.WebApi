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
    internal sealed class BusinessQueryHandler : IRequestHandler<BusinessQuery, Domain.Business>
    {
        private readonly ISrContext _db;

        public BusinessQueryHandler(ISrContext db) =>
            _db = db;

        public async Task<Domain.Business> Handle(BusinessQuery request, CancellationToken cancellationToken)
        {
            var (id, includeShops) = request;

            var business = _db.Businesses.AsQueryable();

            if (includeShops)
                business = business.Include(x => x.Shops);
           
            return await business.FirstOrDefaultAsync(x => x.Id == id, cancellationToken).ConfigureAwait(false);
        }
        
    }
}