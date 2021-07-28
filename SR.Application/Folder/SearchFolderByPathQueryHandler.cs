using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SR.Application.Persistence;

namespace SR.Application.Folder
{
    [UsedImplicitly]
    internal sealed class SearchFolderByPathQueryHandler: IRequestHandler<SearchFolderByPathQuery, Domain.Folder>
    {
        private readonly ISrContext _db;

        public SearchFolderByPathQueryHandler(ISrContext db) =>_db = db;

        public async Task<Domain.Folder> Handle(SearchFolderByPathQuery request, CancellationToken cancellationToken)
        {
            var parts = request.Path.Split(request.Separator, StringSplitOptions.RemoveEmptyEntries);
            var withoutLast = string.Join('/', parts.SkipLast(1));
            var query = string.IsNullOrEmpty(withoutLast) ? null : withoutLast;
            
            return await _db.Folders.FirstOrDefaultAsync(x => 
                x.BusinessId == request.BusinessId && 
                x.Path == query && 
                x.Name.ToUpper() == parts.Last().ToUpper(), cancellationToken).ConfigureAwait(false);
        }
    }
}