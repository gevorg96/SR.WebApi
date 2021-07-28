using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SR.Http.Base
{
    public interface ICqMediator
    {
        IMediator Mediator { get; }
    }

    public interface IGetSingleByQuery<in TGetQuery> : ICqMediator
    {
        async Task<IActionResult> GetByQuery(TGetQuery query, CancellationToken token)
        {
            var result = await Mediator.Send(query, token).ConfigureAwait(false);

            if (result == null)
                return new NotFoundResult();

            return new OkObjectResult(result);
        }
    }

    public interface IUpdateByCommand<in TUpdateCommand>: ICqMediator
    {
        async Task<IActionResult> UpdateByCommand(long id, TUpdateCommand command, CancellationToken token)
        {
            if (id == 0)
                return new BadRequestResult();
            
            await Mediator.Send(command, token).ConfigureAwait(false);
            return new NoContentResult();
        }
    }
}