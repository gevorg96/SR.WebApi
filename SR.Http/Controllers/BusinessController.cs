using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SR.Application.Business;
using SR.Domain;
using SR.Http.Base;

namespace SR.Http.Controllers
{
    [ApiController]
    [Route("api/business")]
    public class BusinessController : Controller, IGetSingleByQuery<BusinessQuery>, IUpdateByCommand<UpdateBusinessCommand>
    {
        public IMediator Mediator { get; }
        public BusinessController(IMediator mediator) => Mediator = mediator;

        
        [HttpGet(Name = "GetBusinesses")]
        [ProducesResponseType(typeof(IReadOnlyCollection<Business>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] BusinessesQuery query,CancellationToken token) =>
            Ok(await Mediator.Send(query, token).ConfigureAwait(false));

        
        [HttpGet("{id}", Name = "GetBusinessQuery")]
        [ProducesResponseType(typeof(Business), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Business), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get([FromRoute] long id, [FromQuery] bool? includeShops, CancellationToken token) =>
            await (this as IGetSingleByQuery<BusinessQuery>).GetByQuery(new BusinessQuery(id, includeShops ?? false), token).ConfigureAwait(false);
        
        
        [HttpPost(Name = "CreateBusiness")]
        [ProducesResponseType(typeof(Business), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post(CreateBusinessCommand command, CancellationToken token) =>
            Created("api/business", await Mediator.Send(command, token).ConfigureAwait(false));

        
        [HttpPut("{id}", Name = "UpdateBusiness")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put([FromRoute] long id, [FromBody] UpdateBusinessCommand command, CancellationToken token) =>
            await (this as IUpdateByCommand<UpdateBusinessCommand>).UpdateByCommand(id, command with { Id = id}, token).ConfigureAwait(false);
        
    }
}