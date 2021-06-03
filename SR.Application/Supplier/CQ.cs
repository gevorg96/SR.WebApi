using System.Collections.Generic;
using MediatR;

namespace SR.Application.Supplier
{
    public record GetSuppliersQuery : IRequest<IReadOnlyCollection<Domain.Supplier>>;

    public record GetSupplierByIdQuery(long Id) : IRequest<Domain.Supplier>;

    public record GetSupplierByNameQuery(string Name) : IRequest<Domain.Supplier>;

    public record CreateSupplierCommand(string Name, string Organization, string Address, string Telephone) : IRequest<Domain.Supplier>;

    public record UpdateSupplierCommand(long Id, string Name, string Organization, string Address, string Telephone) : IRequest<Unit>;
}