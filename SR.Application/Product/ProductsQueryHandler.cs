using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SR.Application.Persistence;

namespace SR.Application.Product
{
    [UsedImplicitly]
    internal sealed class ProductsQueryHandler: IRequestHandler<ProductsQuery, IReadOnlyCollection<ProductView>>
    {
        private readonly ISrContext _db;

        public ProductsQueryHandler(ISrContext db) => _db = db;

        public async Task<IReadOnlyCollection<ProductView>> Handle(ProductsQuery request, CancellationToken cancellationToken)
        {
            var (name, color, size, limit, offset) = request;

            var products = _db.ProductView.AsQueryable();

            if (!string.IsNullOrEmpty(name))
                products = products.Where(x => EF.Functions.Like(x.Name.ToUpper(), $"{name.ToUpper()}%"));
            
            if (!string.IsNullOrEmpty(color))
                products = products.Where(x => EF.Functions.Like(x.Color.ToUpper(), $"{color.ToUpper()}%"));
            
            if (!string.IsNullOrEmpty(size))
                products = products.Where(x => EF.Functions.Like(x.Size.ToUpper(), $"{size.ToUpper()}%"));

            return await products.Skip(limit * offset).Take(limit).ToListAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}