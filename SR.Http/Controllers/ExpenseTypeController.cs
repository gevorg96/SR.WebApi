using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SR.Application.ExpenseType;
using SR.Domain;
using SR.Http.Base;

namespace SR.Http.Controllers
{
    [ApiController]
    [Route("api/expense-type")]
    public class ExpenseTypeController: GetAllAndNameController<
        GetExpenseTypesQuery, 
        GetExpenseTypeByIdQuery, 
        GetExpenseTypeByNameQuery, 
        CreateExpenseTypeCommand>
    {
        protected override string CreatedUrl => "api/expense-type";

        public ExpenseTypeController(IMediator mediator) : base(mediator) {}
        
        [HttpGet(Name = "GetExpenseTypes")]
        [ProducesResponseType(typeof(IReadOnlyCollection<ExpenseType>), StatusCodes.Status200OK)]
        public override async Task<IActionResult> GetAll(CancellationToken token) =>
            await base.GetAll(token).ConfigureAwait(false);

        [HttpGet("{Id}", Name = "GetExpenseTypeById")]
        [ProducesResponseType(typeof(ExpenseType), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public override async Task<IActionResult> GetById(GetExpenseTypeByIdQuery query, CancellationToken token) =>
            await base.GetById(query, token).ConfigureAwait(false);

        [HttpGet("name/{Name}", Name = "GetExpenseTypeByName")]
        [ProducesResponseType(typeof(ExpenseType), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public override async Task<IActionResult> GetByName(GetExpenseTypeByNameQuery query, CancellationToken token) =>
            await base.GetByName(query, token).ConfigureAwait(false);

        [HttpPost(Name = "CreateExpenseType")]
        [ProducesResponseType(typeof(ExpenseType), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public override async Task<IActionResult> Post(CreateExpenseTypeCommand command, CancellationToken token) =>
            await base.Post(command, token).ConfigureAwait(false);
    }
}