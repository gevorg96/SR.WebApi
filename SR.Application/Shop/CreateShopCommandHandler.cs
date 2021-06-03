using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SR.Application.Persistence;
using SR.Domain;

namespace SR.Application.Shop
{
    [UsedImplicitly]
    internal sealed class CreateShopCommandHandler: IRequestHandler<CreateShopCommand, Domain.Shop?>
    {
        private readonly ISrContext _db;

        public CreateShopCommandHandler(ISrContext db) =>
            _db = db;

        public async Task<Domain.Shop?> Handle(CreateShopCommand request, CancellationToken token)
        {
            var (name, address, businessId) = request;
            
            var business = await _db.Businesses
                .FirstOrDefaultAsync(x => x.Id == businessId, token)
                .ConfigureAwait(false);

            Guard.Require(business, businessId, "Бизнес не найден");
            
            var shop = new Domain.Shop
            {
                Name = name,
                Address = address,
                BusinessId = business.Id
            };
            
            var entity = await _db.AddAsync(shop, token).ConfigureAwait(false);
            
            await _db.SaveChangesAsync(token).ConfigureAwait(false);

            return entity.Entity as Domain.Shop;
        }
    }
}