using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SR.Application.Shop;
using SR.Domain;
using SR.Http.Base;

namespace SR.Http.Controllers
{
    [ApiController]
    [Route("api/shop")]
    public class ShopController : GetNameController<GetShopByIdQuery, GetShopByNameQuery, CreateShopCommand>
    {
        protected override string CreatedUrl => "api/shop";

        public ShopController(IMediator mediator): base(mediator) {}
        
        [HttpGet("{Id}", Name = "GetShopById")]
        [ProducesResponseType(typeof(Shop), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public override async Task<IActionResult> GetById(GetShopByIdQuery query, CancellationToken token) =>
            await base.GetById(query, token).ConfigureAwait(false);

        [HttpGet("name/{Name}", Name = "GetShopByName")]
        [ProducesResponseType(typeof(Shop), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public override async Task<IActionResult> GetByName(GetShopByNameQuery query, CancellationToken token) =>
            await base.GetByName(query, token).ConfigureAwait(false);

        [HttpPost(Name = "CreateShop")]
        [ProducesResponseType(typeof(Shop), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public override async Task<IActionResult> Post(CreateShopCommand command, CancellationToken token) =>
            await base.Post(command, token).ConfigureAwait(false);
        
        [HttpPut("{id}", Name = "UpdateShop")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put([FromRoute] long id, [FromBody] UpdateShopCommand command, CancellationToken token)
        {
            if (id == 0)
                return BadRequest();
            
            await Mediator.Send(command with {Id = id}, token).ConfigureAwait(false);

            return NoContent();
        }

        [HttpGet(Name = "GetShopsQuery")]
        [ProducesResponseType(typeof(IReadOnlyCollection<Shop>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromQuery] GetShopsQuery query, CancellationToken token) =>
            Ok(await Mediator.Send(query, token).ConfigureAwait(false));
    }
}