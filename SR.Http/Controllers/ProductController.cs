using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SR.Application.Persistence;
using SR.Application.Product;
using SR.Domain;
using SR.Http.Base;

namespace SR.Http.Controllers
{
    [ApiController]
    [Route("api/product")]
    public class ProductController: GetSaveController<ProductByIdQuery, CreateProductCommand>
    {
        public ProductController(IMediator mediator) : base(mediator) { }
        protected override string CreatedUrl => "api/product";

        [HttpGet("{Id}", Name = "GetProductById")]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public override async Task<IActionResult> GetById(ProductByIdQuery query, CancellationToken token) =>
            await base.GetById(query, token).ConfigureAwait(false);
        
        [HttpGet(Name = "GetProductsQuery")]
        [ProducesResponseType(typeof(IReadOnlyCollection<ProductView>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromQuery] ProductsQuery query, CancellationToken token) =>
            Ok(await Mediator.Send(query, token).ConfigureAwait(false));

        [HttpPost(Name = "CreateProduct")]
        [ProducesResponseType(typeof(Product), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public override async Task<IActionResult> Post(CreateProductCommand command, CancellationToken token) =>
            Created(CreatedUrl, await Mediator.Send(command, token).ConfigureAwait(false));
    }
}