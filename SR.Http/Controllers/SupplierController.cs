using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SR.Application.Supplier;
using SR.Domain;
using SR.Http.Base;

namespace SR.Http.Controllers
{
    [ApiController]
    [Route("api/supplier")]
    public class SupplierController : GetAllAndNameController<GetSuppliersQuery, GetSupplierByIdQuery, GetSupplierByNameQuery, CreateSupplierCommand>
    {
        protected override string CreatedUrl => "api/supplier";

        public SupplierController(IMediator mediator): base(mediator) {}
        
        [HttpGet(Name = "GetSuppliers")]
        [ProducesResponseType(typeof(IReadOnlyCollection<Supplier>), StatusCodes.Status200OK)]
        public override async Task<IActionResult> GetAll(CancellationToken token) =>
            await base.GetAll(token).ConfigureAwait(false);

        [HttpGet("{Id}", Name = "GetSupplierById")]
        [ProducesResponseType(typeof(Supplier), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public override async Task<IActionResult> GetById(GetSupplierByIdQuery query, CancellationToken token) =>
            await base.GetById(query, token).ConfigureAwait(false);

        [HttpGet("name/{Name}", Name = "GetSupplierByName")]
        [ProducesResponseType(typeof(Supplier), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public override async Task<IActionResult> GetByName(GetSupplierByNameQuery query, CancellationToken token) =>
            await base.GetByName(query, token).ConfigureAwait(false);

        [HttpPost(Name = "CreateSupplier")]
        [ProducesResponseType(typeof(Supplier), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public override async Task<IActionResult> Post(CreateSupplierCommand command, CancellationToken token) =>
            await base.Post(command, token).ConfigureAwait(false);
        
        [HttpPut("{id}", Name = "UpdateSupplier")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put([FromRoute] long id, [FromBody] UpdateSupplierCommand command, CancellationToken token)
        {
            if (id == 0)
                return BadRequest();
            
            await Mediator.Send(command with {Id = id}, token).ConfigureAwait(false);

            return NoContent();
        }
    }
}