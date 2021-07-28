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
    public class ExpenseItemController: GetSaveController<ExpenseItemByIdQuery, CreateExpenseItemCommand>, IUpdateByCommand<UpdateExpenseItemCommand>
    {
        protected override string CreatedUrl => "api/expense-item";
        
        public ExpenseItemController(IMediator mediator) : base(mediator) { }

        [HttpGet("{Id}", Name = "GetExpenseItemById")]
        [ProducesResponseType(typeof(ExpenseItem), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public override async Task<IActionResult> GetById(ExpenseItemByIdQuery query, CancellationToken token) =>
            await base.GetById(query, token).ConfigureAwait(false);

        [HttpPost(Name = "CreateExpenseItem")]
        [ProducesResponseType(typeof(ExpenseItem), StatusCodes.Status201Created)]
        public override async Task<IActionResult> Post(CreateExpenseItemCommand command, CancellationToken token) =>
            await base.Post(command, token).ConfigureAwait(false);

        [HttpGet("{ExpenseId}/expense", Name = "GetExpenseItemsByExpense")]
        [ProducesResponseType(typeof(IReadOnlyCollection<ExpenseItem>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromRoute] ExpenseItemsOfExpenseQuery query, CancellationToken token) =>
            Ok(await Mediator.Send(query, token).ConfigureAwait(false));
        
        [HttpPut("{id}", Name = "UpdateExpenseItem")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put([FromRoute] long id, [FromBody] UpdateExpenseItemCommand command, CancellationToken token) =>
            await (this as IUpdateByCommand<UpdateExpenseItemCommand>).UpdateByCommand(id, command with {Id = id}, token).ConfigureAwait(false);
    }
}