using System.Collections.Generic;
using MediatR;

namespace SR.Application.UnitOfMeasure
{ 
    public record CreateUnitOfMeasureCommand(string Name) : IRequest<Domain.UnitOfMeasure>;
    
    public record UnitOfMeasuresQuery : IRequest<IReadOnlyCollection<Domain.UnitOfMeasure>>;
    
    public record UnitOfMeasureByIdQuery(long Id) : IRequest<Domain.UnitOfMeasure>;

    public record UnitOfMeasureByNameQuery(string Name) : IRequest<Domain.UnitOfMeasure>;
}