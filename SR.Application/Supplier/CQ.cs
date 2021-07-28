using System.Collections.Generic;
using MediatR;

namespace SR.Application.Supplier
{
    public record SuppliersQuery : IRequest<IReadOnlyCollection<Domain.Supplier>>;

    public record SupplierByIdQuery(long Id) : IRequest<Domain.Supplier>;

    public record SupplierByNameQuery(string Name) : IRequest<Domain.Supplier>;

    public record CreateSupplierCommand(string Name, string Organization, string Address, string Telephone) : IRequest<Domain.Supplier>;

    public record UpdateSupplierCommand(long Id, string Name, string Organization, string Address, string Telephone) : IRequest<Unit>;
}