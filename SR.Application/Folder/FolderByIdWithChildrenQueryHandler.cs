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
    internal sealed class FolderByIdWithChildrenQueryHandler: IRequestHandler<FolderByIdWithChildrenQuery, FolderModel?>
    {
        private readonly ISrContext _db;
        private readonly IMediator _mediator;
        
        public FolderByIdWithChildrenQueryHandler(ISrContext db, IMediator mediator)
        {
            _db = db;
            _mediator = mediator;
        }

        public async Task<FolderModel?> Handle(FolderByIdWithChildrenQuery request, CancellationToken cancellationToken) =>
            request.Level switch
            {
                DeepLevel.NoLevel => await GetNoLevel(request.Id, cancellationToken).ConfigureAwait(false),
                DeepLevel.All => await GetAllLevels(request.Id, cancellationToken).ConfigureAwait(false),
                _ => null
            };

        private async Task<FolderModel?> GetNoLevel(long id, CancellationToken token)
        {
            var folder = await _db.Folders.FirstOrDefaultAsync(x => x.Id == id, token).ConfigureAwait(false);

            return folder == null ? null : new FolderModel(folder.Id, folder.BusinessId, folder.ParentId, folder.Name);
        }

        private async Task<FolderModel?> GetAllLevels(long id, CancellationToken token)
        {
            var folders = await _mediator.Send(new FoldersQuery<long>("Id", id), token).ConfigureAwait(false);

            var parent = folders?.FirstOrDefault(x => x.Id == id);

            return folders != null && folders.Count > 1
                ? parent?.Map().ParallelMap(folders.ToArray(), token)
                : parent?.Map();
        }
    }
}