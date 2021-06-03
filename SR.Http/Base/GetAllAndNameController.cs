using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SR.Http.Base
{
    public abstract class GetAllAndNameController<TGet, TGetById, TGetByName, TPost>: 
        GetNameController<TGetById, TGetByName, TPost> where TGet : new()
    {
        protected GetAllAndNameController(IMediator mediator) : base(mediator) { }
        
        public virtual async Task<IActionResult> GetAll(CancellationToken token) =>
            Ok(await Mediator.Send(new TGet(), token).ConfigureAwait(false));
    }
}