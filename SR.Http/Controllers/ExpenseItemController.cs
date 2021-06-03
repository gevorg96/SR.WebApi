using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SR.Application.ExpenseItem;
using SR.Domain;
using SR.Http.Base;

namespace SR.Http.Controllers
{
    [ApiController]
    [Route("api/expense-item")]
    public class ExpenseItemController: GetSaveController<GetExpenseItemByIdQuery, CreateExpenseItemCommand>
    {
        public ExpenseItemController(IMediator mediator) : base(mediator) { }
        protected override string CreatedUrl => "api/expense-item";

        [HttpGet("{Id}", Name = "GetExpenseItemById")]
        [ProducesResponseType(typeof(ExpenseItem), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public override async Task<IActionResult> GetById(GetExpenseItemByIdQuery query, CancellationToken token) =>
            await base.GetById(query, token).ConfigureAwait(false);

        [HttpPost(Name = "CreateExpenseItem")]
        [ProducesResponseType(typeof(ExpenseItem), StatusCodes.Status201Created)]
        public override async Task<IActionResult> Post(CreateExpenseItemCommand command, CancellationToken token) =>
            await base.Post(command, token).ConfigureAwait(false);

        [HttpGet("{ExpenseId}/expense", Name = "GetExpenseItemsByExpense")]
        [ProducesResponseType(typeof(IReadOnlyCollection<ExpenseItem>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromRoute] GetExpenseItemsOfExpenseQuery query, CancellationToken token) =>
            Ok(await Mediator.Send(query, token).ConfigureAwait(false));
        
        [HttpPut("{id}", Name = "UpdateExpenseItem")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put([FromRoute] long id, [FromBody] UpdateExpenseItem command, CancellationToken token)
        {
            if (id == 0)
                return BadRequest();
            
            await Mediator.Send(command with {Id = id}, token).ConfigureAwait(false);

            return NoContent();
        }
    }
}