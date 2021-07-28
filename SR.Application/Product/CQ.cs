using System.Collections.Generic;
using MediatR;
using SR.Application.Persistence;

namespace SR.Application.Product
{
    public record ProductsQuery(string? Name, string? Color, string? Size, int Limit = 10, int Offset = 0) : IRequest<IReadOnlyCollection<ProductView>>;

    public record ProductByIdQuery(long Id) : IRequest<Domain.Product>;

    public record ProductProps(string? Color, string? Size, long UoMId, long? FolderId);
    
    public record CreateProductCommand(string Name, long BusinessId, long? SupplierId, IEnumerable<ProductProps>? ProductProperties) 
        : IRequest<Domain.Product>;
}