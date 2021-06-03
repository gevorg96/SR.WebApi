using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SR.Application.Expense;
using SR.Application.UnitOfMeasure;
using SR.Domain;
using SR.Http.Base;

namespace SR.Http.Controllers
{
    [ApiController]
    [Route("api/expense")]
    public class ExpenseController: GetSaveController<GetExpenseByIdQuery, CreateExpenseCommand>
    {
        public ExpenseController(IMediator mediator) : base(mediator) { }

        protected override string CreatedUrl => "api/expense";
        
        [HttpGet("{Id}/{WithExpenseItems}", Name = "GetExpenseById")]
        [ProducesResponseType(typeof(Expense), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public override async Task<IActionResult> GetById(GetExpenseByIdQuery query, CancellationToken token) =>
            await base.GetById(query, token).ConfigureAwait(false);
        
        [HttpPost(Name = "CreateExpense")]
        [ProducesResponseType(typeof(Expense), StatusCodes.Status201Created)]
        public override async Task<IActionResult> Post(CreateExpenseCommand command, CancellationToken token) =>
            await base.Post(command, token).ConfigureAwait(false);
        
        [HttpGet(Name = "GetExpensesQuery")]
        [ProducesResponseType(typeof(IReadOnlyCollection<Expense>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromQuery] GetExpenseQuery query, CancellationToken token) =>
            Ok(await Mediator.Send(query, token).ConfigureAwait(false));
    }
}