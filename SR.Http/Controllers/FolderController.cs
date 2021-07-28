using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SR.Application.Folder;
using SR.Http.Base;

namespace SR.Http.Controllers
{    
    [ApiController]
    [Route("api/folder")]
    public class FolderController: Controller, IGetSingleByQuery<FolderByIdWithChildrenQuery>
    {
        public IMediator Mediator { get; }

        public FolderController(IMediator mediator) => Mediator = mediator;

        [HttpGet("{id}", Name = "GetFolder")]
        [ProducesResponseType(typeof(FolderModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] long id, [FromQuery] DeepLevel level, CancellationToken token) =>
            await (this as IGetSingleByQuery<FolderByIdWithChildrenQuery>).GetByQuery(new FolderByIdWithChildrenQuery(id, level), token).ConfigureAwait(false);

        [HttpGet(Name = "GetFoldersByName")]
        [ProducesResponseType(typeof(FolderModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByName([FromQuery] SearchFoldersByNameQuery query, CancellationToken token)
        {
            var result = await Mediator.Send(query, token).ConfigureAwait(false);
            if (result == null || !result.Any())
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(FolderModel), StatusCodes.Status201Created)]
        public async Task<IActionResult> Save([FromBody] CreateFolderCommand command, CancellationToken token) =>
            Created("/api/folder", await Mediator.Send(command, token).ConfigureAwait(false));
    }
}