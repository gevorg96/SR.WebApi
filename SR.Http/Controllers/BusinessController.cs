using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SR.Application.Business;
using SR.Domain;

namespace SR.Http.Controllers
{
    [ApiController]
    [Route("api/business")]
    public class BusinessController : Controller
    {
        private readonly IMediator _mediator;
        public BusinessController(IMediator mediator) =>
            _mediator = mediator;

        [HttpGet("{IncludeShops}", Name = "GetBusinesses")]
        [ProducesResponseType(typeof(IReadOnlyCollection<Business>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromRoute] GetBusinessesQuery query,CancellationToken token) =>
            Ok(await _mediator.Send(query, token).ConfigureAwait(false));

        [HttpGet(Name = "GetBusinessQuery")]
        [ProducesResponseType(typeof(Business), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Business), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get([FromQuery] GetBusinessQuery query, CancellationToken token)
        {
            var item = await _mediator.Send(query, token).ConfigureAwait(false);
            if (item == null)
                return NotFound();

            return Ok(item);
        }

        [HttpPost(Name = "CreateBusiness")]
        [ProducesResponseType(typeof(Business), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post(CreateBusinessCommand command, CancellationToken token) =>
            Created("api/business", await _mediator.Send(command, token).ConfigureAwait(false));

        [HttpPut("{id}", Name = "UpdateBusiness")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put([FromRoute] long id, [FromBody] UpdateBusinessCommand command, CancellationToken token)
        {
            if (id == 0)
                return BadRequest();
            
            await _mediator.Send(command with {Id = id}, token).ConfigureAwait(false);
            return NoContent();
        }
    }
}