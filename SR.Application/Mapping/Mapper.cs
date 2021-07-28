using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EmitMapper;
using SR.Application.Folder;

namespace SR.Application.Mapping
{
    public static class Mapper
    {
        public static FolderModel Map(this Domain.Folder folder) => new(folder.Id, folder.BusinessId, folder.ParentId, folder.Name, folder.Path);
        
        public static FolderModel ParallelMap(this FolderModel model, IReadOnlyCollection<Domain.Folder> folders, CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return model;
            
            model.Children = folders.Where(x => x.ParentId == model.Id).Select(Map).ToList();
            Parallel.ForEach(model.Children, folder => ParallelMap(folder, folders, token));
            
            return model;
        }
    }
}