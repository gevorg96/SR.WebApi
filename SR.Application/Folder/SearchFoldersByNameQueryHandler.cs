using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SR.Application.Mapping;
using SR.Application.Persistence;

namespace SR.Application.Folder
{
    [UsedImplicitly]
    internal sealed class SearchFoldersByNameQueryHandler: IRequestHandler<SearchFoldersByNameQuery, IReadOnlyCollection<FolderModel>?>
    {
        private readonly ISrContext _db;
        
        public SearchFoldersByNameQueryHandler(ISrContext db, IMediator mediator) => _db = db;

        public async Task<IReadOnlyCollection<FolderModel>?> Handle(SearchFoldersByNameQuery request, CancellationToken token)
        {
            var (businessId, createTree, folderName) = request;
            var folders = await _db.Folders.Where(x => x.BusinessId == businessId).ToListAsync(token).ConfigureAwait(false);

            if (folders == null || !folders.Any())
                return null;
            
            if (!string.IsNullOrEmpty(folderName))
                return GetFilteredFolders(folders, folderName).Select(x => x.Map()).ToArray();
            
            return createTree 
                ? folders.Where(x => x.ParentId == null).Select(x => x.Map().ParallelMap(folders.ToArray(), token)).ToArray() 
                : folders.Select(x => x.Map()).ToArray();
        }
        
        private static IEnumerable<Domain.Folder> GetFilteredFolders(IEnumerable<Domain.Folder> folders, string criteria)
        {
            var result = new ConcurrentBag<Domain.Folder>();

            Parallel.ForEach(folders, folder =>
            {
                if (folder.Name.StartsWith(criteria, StringComparison.InvariantCultureIgnoreCase))
                    result.Add(folder);
            });

            return result.ToList();
        }
    }
}