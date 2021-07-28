using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SR.Application.Mapping;
using SR.Application.Persistence;

namespace SR.Application.Folder
{
    public class CreateFolderCommandHandler: IRequestHandler<CreateFolderCommand, FolderModel>
    {
        private readonly ISrContext _db;
        private readonly IMediator _mediator;
        
        public CreateFolderCommandHandler(ISrContext db, IMediator mediator)
        {
            _db = db;
            _mediator = mediator;
        }

        public async Task<FolderModel> Handle(CreateFolderCommand request, CancellationToken cancellationToken)
        {
            var (businessId, path, folderName, parentId, separator) = request;
            
            var folder = new Domain.Folder
            {
                BusinessId = businessId,
                Path = string.IsNullOrEmpty(path) ? null : path,
                Name = folderName
            };

            if (parentId.HasValue) 
                folder.ParentId = parentId;
            
            else if (!string.IsNullOrEmpty(path))
            {
                var parent = await _mediator.Send(new SearchFolderByPathQuery(businessId, path, separator), cancellationToken).ConfigureAwait(false);
                folder.ParentId = parent.Id;
            }
            
            await _db.Folders.AddAsync(folder, cancellationToken).ConfigureAwait(false);
            await _db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return folder.Map();
        }
    }
}