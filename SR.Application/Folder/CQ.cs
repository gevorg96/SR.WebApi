using System.Collections.Generic;
using MediatR;

namespace SR.Application.Folder
{
    public record FolderModel(long Id, long BusinessId, long? ParentId, string Name, List<FolderModel>? Children = null)
    {
        public List<FolderModel>? Children { get; set; } = Children;
    }
    
    public record FolderByIdWithChildrenQuery(long Id, DeepLevel Level = DeepLevel.NoLevel) : IRequest<FolderModel?>;

    public record FoldersQuery<T>(string Field, T Value) : IRequest<IReadOnlyCollection<Domain.Folder>?>;
    
    public record SearchFoldersByNameQuery(long BusinessId, bool CreateTree, string? FolderName) : IRequest<IReadOnlyCollection<FolderModel>?>;
    
    public enum DeepLevel: uint
    {
        NoLevel = 0,
        All
    }
}