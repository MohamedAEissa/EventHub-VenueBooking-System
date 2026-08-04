using EventHub.Application.Common.InterFaces;
using EventHub.Application.Features.Events.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.TicketType.Queries
{
    public record GetTicketTypesByEventIdQuery(Guid EventId) : IRequest<List<ResponseTicketTypeDto>>;

    public class GetTicketTypesByEventIdQueryHandler : IRequestHandler<GetTicketTypesByEventIdQuery, List<ResponseTicketTypeDto>>
    {
        private readonly IApplicationDbContext _dbContext;
        public GetTicketTypesByEventIdQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ResponseTicketTypeDto>> Handle(GetTicketTypesByEventIdQuery request, CancellationToken cancellationToken)
        {
           return await _dbContext.Tickets.AsNoTracking().Where(e=>e.EventId==request.EventId).Select(t=>new ResponseTicketTypeDto
           {
               Id = t.Id,
               EventId = t.EventId,
               Type = t.Type,
               Price = t.Price,
               TotalQuantity = t.TotalQuantity,
               AvailableQuantity = t.AvailableQuantity
           }).ToListAsync(cancellationToken);
        }
    }
}
