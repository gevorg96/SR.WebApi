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
    internal sealed class UpdateShopCommandHandler: IRequestHandler<UpdateShopCommand, Unit>
    {
        private readonly ISrContext _db;

        public UpdateShopCommandHandler(ISrContext db) =>
            _db = db;

        public async Task<Unit> Handle(UpdateShopCommand request, CancellationToken token)
        {
            var (id, name, address) = request;
            
            var shop = await _db.Shops
                .FirstOrDefaultAsync(x => x.Id == id, token)
                .ConfigureAwait(false);

            Guard.Require(shop, id, "Магазин не найден");
  
            shop.Name = name;
            shop.Address = address;

            _db.Shops.Update(shop);
            await _db.SaveChangesAsync(token).ConfigureAwait(false);
            
            return Unit.Value;
        }
    }
}