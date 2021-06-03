using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SR.Application.Persistence;

namespace SR.Application.Business
{
    [UsedImplicitly]
    internal sealed class CreateBusinessCommandHandler : IRequestHandler<CreateBusinessCommand, Domain.Business?>
    {
        private readonly ISrContext _db;

        public CreateBusinessCommandHandler(ISrContext db) =>
            _db = db;

        public async Task<Domain.Business?> Handle(CreateBusinessCommand request, CancellationToken token)
        {
            var (name, telephone, shops) = request;
            var existing = await _db.Businesses
                .FirstOrDefaultAsync(x => x.Name.ToUpper() == name.ToUpper(), token)
                .ConfigureAwait(false);

            if (existing != null)
                throw new ArgumentException("Бизнес с таким именем уже существует");

            var business = new Domain.Business
            {
                Name = name,
                Telephone = telephone
            };

            var transaction = await _db.Database.BeginTransactionAsync(token).ConfigureAwait(false);

            try
            {
                var entity = await _db.AddAsync(business, token).ConfigureAwait(false);

                if (!(entity.Entity is Domain.Business businessEntity))
                    throw new ArgumentException("Не удалось сохранить бизнес");

                if (shops != null && shops.Any())
                    foreach (var shop in shops)
                    {
                        shop.BusinessId = businessEntity.Id;
                        await _db.AddAsync(shop, token).ConfigureAwait(false);
                    }

                await _db.SaveChangesAsync(token).ConfigureAwait(false);
                transaction.Commit();
                
                return businessEntity;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}