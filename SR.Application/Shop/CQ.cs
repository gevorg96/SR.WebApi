using System.Collections.Generic;
using MediatR;

namespace SR.Application.Shop
{
    public record GetShopByIdQuery(long Id) : IRequest<Domain.Shop>;

    public record GetShopByNameQuery(string Name) : IRequest<Domain.Shop>;

    public record GetShopsQuery(long? BusinessId) : IRequest<IReadOnlyCollection<Domain.Shop>>;

    public record CreateShopCommand(string Name, string Address, long BusinessId) : IRequest<Domain.Shop>;

    public record UpdateShopCommand(long Id, string Name, string Address) : IRequest<Unit>;
}