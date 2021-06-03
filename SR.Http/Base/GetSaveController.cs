using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SR.Http.Base
{
    public abstract class GetSaveController<TGetById, TPost> : Controller
    {
        protected abstract string CreatedUrl { get; }
        
        protected readonly IMediator Mediator;

        protected GetSaveController(IMediator mediator) =>
            Mediator = mediator;

        public virtual async Task<IActionResult> GetById([FromRoute] TGetById query, CancellationToken token) =>
            await OkOrNotFound(() => Mediator.Send(query, token)).ConfigureAwait(false);
        
        public virtual async Task<IActionResult> Post([FromBody] TPost command, CancellationToken token)
        {
            var item = await Mediator.Send(command, token).ConfigureAwait(false);

            return Created(CreatedUrl, item);
        }

        protected async Task<IActionResult> OkOrNotFound<T>(Func<Task<T>> func)
        {
            var entity = await func().ConfigureAwait(false);
            return entity!= null ? Ok(entity) : NotFound();
        }
    }
}