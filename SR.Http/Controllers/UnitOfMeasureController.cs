using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SR.Application.UnitOfMeasure;
using SR.Domain;
using SR.Http.Base;

namespace SR.Http.Controllers
{
    [ApiController]
    [Route("api/uom")]
    public class UnitOfMeasureController: GetAllAndNameController<
        GetUnitOfMeasuresQuery, 
        GetUnitOfMeasureByIdQuery, 
        GetUnitOfMeasureByNameQuery, 
        CreateUnitOfMeasureCommand>
    {
        public UnitOfMeasureController(IMediator mediator) : base(mediator) {}
        protected override string CreatedUrl => "api/uom";

        [HttpGet(Name = "GetUnitOfMeasures")]
        [ProducesResponseType(typeof(IReadOnlyCollection<UnitOfMeasure>), StatusCodes.Status200OK)]
        public override async Task<IActionResult> GetAll(CancellationToken token) =>
            await base.GetAll(token).ConfigureAwait(false);

        [HttpGet("{Id}", Name = "GetUnitOfMeasureById")]
        [ProducesResponseType(typeof(UnitOfMeasure), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public override async Task<IActionResult> GetById(GetUnitOfMeasureByIdQuery query, CancellationToken token) =>
            await base.GetById(query, token).ConfigureAwait(false);

        [HttpGet("name/{Name}", Name = "GetUnitOfMeasureByName")]
        [ProducesResponseType(typeof(UnitOfMeasure), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public override async Task<IActionResult> GetByName(GetUnitOfMeasureByNameQuery query, CancellationToken token) =>
            await base.GetByName(query, token).ConfigureAwait(false);

        [HttpPost(Name = "CreateUnitOfMeasure")]
        [ProducesResponseType(typeof(UnitOfMeasure), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public override async Task<IActionResult> Post(CreateUnitOfMeasureCommand command, CancellationToken token) =>
            await base.Post(command, token).ConfigureAwait(false);
    }
}