using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SR.Application.Persistence;

namespace SR.Application.Supplier
{
    [UsedImplicitly]
    internal sealed class CreateSupplierCommandHandler: IRequestHandler<CreateSupplierCommand, Domain.Supplier?>
    {
        private readonly ISrContext _db;

        public CreateSupplierCommandHandler(ISrContext db) =>
            _db = db;

        public async Task<Domain.Supplier?> Handle(CreateSupplierCommand request, CancellationToken token)
        {
            var existing = await _db.Suppliers
                .FirstOrDefaultAsync(x =>
                    string.IsNullOrEmpty(x.Address) && string.IsNullOrEmpty(request.Address) && 
                    x.Organization.ToUpper() == request.Organization.ToUpper() || 
                    x.Address!.ToUpper() == request.Address.ToUpper() && 
                    x.Organization.ToUpper() == request.Organization.ToUpper(), token)
                .ConfigureAwait(false);

            if (existing != null)
                throw new ArgumentException("Поставщик с таким адресом и/или организацией уже существует");
            
            var supplier = new Domain.Supplier
            {
                Name = request.Name,
                Address = request.Address,
                Telephone = request.Telephone,
                Organization = request.Organization
            };
            
            var entity = await _db.AddAsync(supplier, token).ConfigureAwait(false);
            
            await _db.SaveChangesAsync(token).ConfigureAwait(false);

            return entity.Entity as Domain.Supplier;
        }
    }
}