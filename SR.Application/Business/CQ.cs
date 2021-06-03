using System.Collections.Generic;
using MediatR;

namespace SR.Application.Business
{
    public record GetBusinessesQuery(bool IncludeShops) : IRequest<IReadOnlyCollection<Domain.Business>>;

    public record GetBusinessQuery(long? Id, string? Name, bool? IncludeShops) : IRequest<Domain.Business>;
    
    public record CreateBusinessCommand(string Name, string Telephone, IEnumerable<Domain.Shop>? Shops) : IRequest<Domain.Business>;

    public record UpdateBusinessCommand(long Id, string Name, string Telephone) : IRequest<Unit>;
}