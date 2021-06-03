﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SR.Application.Persistence;

namespace SR.Application.Supplier
{
    [UsedImplicitly]
    internal sealed class GetSuppliersQueryHandler: IRequestHandler<GetSuppliersQuery, IReadOnlyCollection<Domain.Supplier>>
    {
        private readonly ISrContext _db;

        public GetSuppliersQueryHandler(ISrContext db) =>
            _db = db;

        public async Task<IReadOnlyCollection<Domain.Supplier>> Handle(GetSuppliersQuery request, CancellationToken cancellationToken) => 
            await _db.Suppliers.ToListAsync(cancellationToken).ConfigureAwait(false);
    }
}