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
    public record GetEventByIdQuery(Guid EventId) : IRequest<ResponseEventDto>;
    public class GetEventByIdQueryHandler : IRequestHandler<GetEventByIdQuery, ResponseEventDto>
    {
        private readonly IApplicationDbContext _dbContext;

        public GetEventByIdQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ResponseEventDto> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
        {
            var eventDto = await _dbContext.Events
            .AsNoTracking()
            .Where(e => e.Id == request.EventId)
            .Select(e => new ResponseEventDto
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                EventDate = e.EventDate,
                HallId = e.HallId,
                HallName = e.Hall.Name,
                VenueName = e.Hall.Venue.Name,
                TicketTypes = e.Tickets.Select(tt => new ResponseTicketTypeDto
                {
                    Id = tt.Id,
                    EventId = e.Id,
                    Type = tt.Type,
                    Price = tt.Price,
                    TotalQuantity = tt.TotalQuantity,
                    AvailableQuantity = tt.AvailableQuantity
                }).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

            if (eventDto is null)
                throw new KeyNotFoundException($"Event with ID '{request.EventId}' was not found.");

            return eventDto;
        }
    }
}
