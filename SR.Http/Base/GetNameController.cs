using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SR.Http.Base
{
    public abstract class GetNameController<TGetById, TGetByName, TPost> : GetSaveController<TGetById, TPost>
    {
        protected GetNameController(IMediator mediator) : base(mediator) {}

        public virtual async Task<IActionResult> GetByName([FromRoute] TGetByName query, CancellationToken token) =>
            await OkOrNotFound(() => Mediator.Send(query, token)).ConfigureAwait(false);
    }
}