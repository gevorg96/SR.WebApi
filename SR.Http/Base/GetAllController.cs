using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SR.Http.Base
{
    public abstract class GetAllController<TGet, TGetById, TPost>: GetSaveController<TGetById, TPost> where TGet : new()
    {
        protected GetAllController(IMediator mediator) : base(mediator) { }
        
        public virtual async Task<IActionResult> GetAll(CancellationToken token) =>
            Ok(await Mediator.Send(new TGet(), token).ConfigureAwait(false));
    }
}