using EventHub.Application.Common.InterFaces;
using EventHub.Application.Features.Events.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Events.Queries
{
    public record GetAllEventsQuery : IRequest< List<ResponseEventDto>>;
    public class GetAllEventsQueryHandler : IRequestHandler<GetAllEventsQuery, List<ResponseEventDto>>
    {
        private readonly IApplicationDbContext _dbContext;

        public GetAllEventsQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<ResponseEventDto>> Handle(GetAllEventsQuery request, CancellationToken cancellationToken)
        {
            return _dbContext.Events
                 .AsNoTracking()
                 .Include(e => e.Hall)
                     .ThenInclude(h => h.Venue)
                 .Include(e => e.Tickets)
                 .Select(e => new ResponseEventDto
                 {
                     Id=e.Id,
                     Title = e.Title,
                     Description = e.Description,
                     EventDate = e.EventDate,
                     HallId = e.HallId,
                     HallName = e.Hall.Name,
                     VenueName = e.Hall.Venue.Name,
                     TicketTypes=e.Tickets.Select(tt=> new ResponseTicketTypeDto
                     {
                         Id = tt.Id,
                         EventId = e.Id,
                         Type = tt.Type,
                         Price = tt.Price,
                         TotalQuantity = tt.TotalQuantity,
                         AvailableQuantity = tt.AvailableQuantity
                     }).ToList()

                 }).ToListAsync(cancellationToken);
        }
    }
}
