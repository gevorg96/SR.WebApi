using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SR.Application.Persistence;
using SR.Domain;

namespace SR.Application.Business
{
    [UsedImplicitly]
    internal sealed class UpdateBusinessCommandHandler: IRequestHandler<UpdateBusinessCommand, Unit>
    {
        private readonly ISrContext _db;

        public UpdateBusinessCommandHandler(ISrContext db) =>
            _db = db;

        public async Task<Unit> Handle(UpdateBusinessCommand request, CancellationToken token)
        {
            var (id, name, telephone) = request;
            
            var business = await _db.Businesses
                .FirstOrDefaultAsync(x => x.Id == id, token)
                .ConfigureAwait(false);

            Guard.Require(business, id, "Бизнес не найден");
            
            business.Name = name;
            business.Telephone = telephone;

            _db.Businesses.Update(business);
            await _db.SaveChangesAsync(token).ConfigureAwait(false);
            
            return Unit.Value;
        }
    }
}