using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SR.Application.Persistence;
using SR.Domain;

namespace SR.Application.Product
{
    [UsedImplicitly]
    internal sealed class CreateProductCommandHandler: IRequestHandler<CreateProductCommand, Domain.Product?>
    {
        private readonly ISrContext _db;
        
        public CreateProductCommandHandler(ISrContext db) =>
            _db = db;

        public async Task<Domain.Product?> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var (name, businessId, supplierId, productProperties) = request;

            var business = await _db.Businesses
                .FirstOrDefaultAsync(x => x.Id == businessId, cancellationToken)
                .ConfigureAwait(false);

            Guard.Require(business, businessId, "Бизнес не найден");

            var productPropsEnumerable = productProperties.ToList();
            var product = new Domain.Product
            {
                BusinessId = businessId,
                Name = name,
                SupplierId = supplierId,
                ProductProperties = productProperties != null && productPropsEnumerable.Any() 
                    ? Transform(productPropsEnumerable).ToList() 
                    : null
            };

            var transaction = await _db.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                var entity = await _db.AddAsync(product, cancellationToken).ConfigureAwait(false);
                await _db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                
                // if (!(entity.Entity is Domain.Product productEntity))
                //     throw new ArgumentException("Не удалось сохранить товар");
                //
                // if (productProperties != null && productProperties.Any())
                // {
                //     foreach (var productProperty in productProperties)
                //     {
                //         var property = new ProductProperty
                //         {
                //             ProductId = productEntity.Id,
                //             UoMId = productProperty.UoMId,
                //             FolderId = productProperty.FolderId,
                //             Color = productProperty.Color,
                //             Size = productProperty.Size
                //         };
                //         
                //         await _db.AddAsync(property, cancellationToken).ConfigureAwait(false);
                //     }
                // }

                // await _db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                
                transaction.Commit();
                return entity.Entity as Domain.Product;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw;
            }
        }

        private IEnumerable<ProductProperty> Transform(IEnumerable<ProductProps> productProperties) =>
            productProperties.Select(productProperty => new ProductProperty
            {
                UoMId = productProperty.UoMId,
                FolderId = productProperty.FolderId,
                Color = productProperty.Color,
                Size = productProperty.Size
            });
        
    }
}