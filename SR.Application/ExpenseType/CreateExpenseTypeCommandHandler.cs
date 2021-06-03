using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SR.Application.Persistence;

namespace SR.Application.ExpenseType
{
    [UsedImplicitly]
    internal sealed class CreateExpenseTypeCommandHandler : IRequestHandler<CreateExpenseTypeCommand, Domain.ExpenseType?>
    {
        private readonly ISrContext _db;

        public CreateExpenseTypeCommandHandler(ISrContext db) =>
            _db = db;

        public async Task<Domain.ExpenseType?> Handle(CreateExpenseTypeCommand request, CancellationToken token)
        {
            var existing = await _db.ExpenseTypes
                .FirstOrDefaultAsync(x => x.Name.ToUpper() == request.Name.ToUpper(), token)
                .ConfigureAwait(false);
            
            if (existing != null)
                throw new ArgumentException("Тип застрат с таким наименованием уже существует");
            
            var entity = await _db.AddAsync(new Domain.ExpenseType{Name = request.Name}, token).ConfigureAwait(false);
            
            await _db.SaveChangesAsync(token).ConfigureAwait(false);

            return entity.Entity as Domain.ExpenseType;
        }
    }
}