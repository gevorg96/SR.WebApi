using System.Collections.Generic;
using MediatR;

namespace SR.Application.UnitOfMeasure
{ 
    public record CreateUnitOfMeasureCommand(string Name) : IRequest<Domain.UnitOfMeasure>;
    
    public record GetUnitOfMeasuresQuery : IRequest<IReadOnlyCollection<Domain.UnitOfMeasure>>;
    
    public record GetUnitOfMeasureByIdQuery(long Id) : IRequest<Domain.UnitOfMeasure>;

    public record GetUnitOfMeasureByNameQuery(string Name) : IRequest<Domain.UnitOfMeasure>;
}