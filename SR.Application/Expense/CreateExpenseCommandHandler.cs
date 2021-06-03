using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SR.Application.Persistence;
using SR.Domain;

namespace SR.Application.Expense
{
    [UsedImplicitly]
    internal sealed class CreateExpenseCommandHandler: IRequestHandler<CreateExpenseCommand, Domain.Expense?>
    {
        private readonly ISrContext _db;

        public CreateExpenseCommandHandler(ISrContext db) =>
            _db = db;

        public async Task<Domain.Expense?> Handle(CreateExpenseCommand request, CancellationToken token)
        {
            var (reportDate, amount, businessId, shopId, items) = request;

            var business = await _db.Businesses
                .FirstOrDefaultAsync(x => x.Id == businessId, token).ConfigureAwait(false);

            var shop = await _db.Shops
                .FirstOrDefaultAsync(x => x.Id == shopId, token).ConfigureAwait(false);
            
            business = Guard.Require(business, businessId, "Бизнес не найден");
            shop = Guard.Require(shop, shopId, "Бизнес не найден");
            
            var expense = new Domain.Expense
            {
               Amount = amount,
               ReportDate = reportDate,
               Business = business,
               Shop = shop,
               BusinessId = businessId,
               ShopId = shopId
            };

            var transaction = await _db.Database.BeginTransactionAsync(token).ConfigureAwait(false);
            
            try
            {
                var entity = await _db.AddAsync(expense, token).ConfigureAwait(false);

                if (!(entity.Entity is Domain.Expense expenseEntity))
                    throw new ArgumentException("Не удалось сохранить затраты");
                    
                foreach (var item in items)
                {
                    item.ExpenseId = expenseEntity.Id;
                    await _db.AddAsync(item, token).ConfigureAwait(false);
                }
            
                await _db.SaveChangesAsync(token).ConfigureAwait(false);
                transaction.Commit();

                return expenseEntity;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}