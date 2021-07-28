using System.Collections.Generic;
using MediatR;

namespace SR.Application.Business
{
    public record BusinessesQuery(bool IncludeShops, string? Name) : IRequest<IReadOnlyCollection<Domain.Business>>;

    public record BusinessQuery(long Id, bool IncludeShops) : IRequest<Domain.Business>;
    
    public record CreateBusinessCommand(string Name, string Telephone, IEnumerable<Domain.Shop>? Shops) : IRequest<Domain.Business>;

    public record UpdateBusinessCommand(long Id, string Name, string Telephone) : IRequest<Unit>;
}