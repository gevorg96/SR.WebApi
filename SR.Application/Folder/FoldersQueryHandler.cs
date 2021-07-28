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
    internal sealed class FoldersQueryHandler: IRequestHandler<FoldersQuery<long>, IReadOnlyCollection<Domain.Folder>?>
    {
        private readonly ISrContext _db;

        public FoldersQueryHandler(ISrContext db) => _db = db;

        public async Task<IReadOnlyCollection<Domain.Folder>?> Handle(FoldersQuery<long> request, CancellationToken token)
        {
            var (field, value) = request;
            
            var folders = await _db.Folders.FromSqlRaw(
                $"with recursive folder_tree as ("
                + " select f.\"Id\", f.\"BusinessId\", f.\"ParentId\", f.\"Name\", f.\"Path\"" 
                + "     from \"Folders\" f"
                + "     where \"" + field +"\" = " + value
                + " union all "
                + "     select child.\"Id\", child.\"BusinessId\", child.\"ParentId\", child.\"Name\", child.\"Path\""
                + "     from \"Folders\" child" 
                + "     join folder_tree parent on parent.\"Id\" = child.\"ParentId\""
                + ") select * from folder_tree;").ToListAsync(token);

            return folders == null || !folders.Any() ? null : folders;
        }
    }
}