using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SR.Application.Persistence;

namespace SR.Application.UnitOfMeasure
{
    [UsedImplicitly]
    internal sealed class CreateUnitOfMeasureCommandHandler : IRequestHandler<CreateUnitOfMeasureCommand, Domain.UnitOfMeasure?>
    {
        private readonly ISrContext _db;

        public CreateUnitOfMeasureCommandHandler(ISrContext db) =>
            _db = db;

        public async Task<Domain.UnitOfMeasure?> Handle(CreateUnitOfMeasureCommand request, CancellationToken token)
        {
            var existing = await _db.UnitOfMeasures
                .FirstOrDefaultAsync(x => x.Name.ToUpper() == request.Name.ToUpper(), token)
                .ConfigureAwait(false);

            if (existing != null)
                throw new ArgumentException("Единица измерения с таким наименованием имеется");
            
            var entity = await _db.AddAsync(new Domain.UnitOfMeasure{Name = request.Name}, token).ConfigureAwait(false);
            
            await _db.SaveChangesAsync(token).ConfigureAwait(false);

            return entity.Entity as Domain.UnitOfMeasure;
        }
    }
}