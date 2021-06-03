using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SR.Application.Persistence;

namespace SR.Application.Expense
{
    [UsedImplicitly]
    internal sealed class GetExpenseQueryHandler: IRequestHandler<GetExpenseQuery, IReadOnlyCollection<Domain.Expense>>
    {
        private readonly ISrContext _db;

        public GetExpenseQueryHandler(ISrContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyCollection<Domain.Expense>> Handle(GetExpenseQuery request, CancellationToken cancellationToken)
        {
            var (businessId, shopId, withExpenseItems, on, from, to) = request;
            if (businessId < 1)
                throw new ArgumentException("Необходимо указать бизнес");
            
            var expenses = _db.Expenses.AsQueryable().Where(x => x.BusinessId == businessId);

            if (shopId.HasValue)
                expenses = expenses.Where(x => x.ShopId == shopId);
            if (on.HasValue)
                expenses = expenses.Where(x => x.ReportDate == on);
            
            if(from.HasValue)
                expenses = expenses.Where(x => x.ReportDate >= from);
            if (to.HasValue)
                expenses = expenses.Where(x => x.ReportDate <= to);

            if (withExpenseItems.HasValue && withExpenseItems.Value)
                expenses = expenses.Include(x => x.ExpenseItems).ThenInclude(x => x.ExpenseType);

            return await expenses.ToListAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}