using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SR.Application.Persistence;
using SR.Domain;

namespace SR.Application.Supplier
{
    [UsedImplicitly]
    internal sealed class UpdateSupplierCommandHandler: IRequestHandler<UpdateSupplierCommand, Unit>
    {
        private readonly ISrContext _db;

        public UpdateSupplierCommandHandler(ISrContext db) =>
            _db = db;

        public async Task<Unit> Handle(UpdateSupplierCommand request, CancellationToken token)
        {
            var (id, name, organization, address, telephone) = request;
            
            var supplier = await _db.Suppliers
                .FirstOrDefaultAsync(x => x.Id == id, token)
                .ConfigureAwait(false);

            Guard.Require(supplier, id, "Поставщик не найден");

            supplier.Name = name;
            supplier.Address = address;
            supplier.Organization = organization;
            supplier.Telephone = telephone;

            _db.Suppliers.Update(supplier);
            await _db.SaveChangesAsync(token).ConfigureAwait(false);
            
            return Unit.Value;
        }
    }
}