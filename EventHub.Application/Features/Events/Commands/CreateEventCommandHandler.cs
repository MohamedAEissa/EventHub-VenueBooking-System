using EventHub.Application.Common.InterFaces;
using EventHub.Application.Features.Events.Dtos;
using EventHub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Events.Commands
{
    public class CreateEventCommandHandler : IRequestHandler<CreateEventDto, ResponseEventDto>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserServie;

        public CreateEventCommandHandler(IApplicationDbContext dbContext , ICurrentUserService currentUserServie)
        {
            _dbContext = dbContext;
            _currentUserServie = currentUserServie;
        }
        public async Task<ResponseEventDto> Handle(CreateEventDto request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_currentUserServie.UserId,out var currentUserId))
                 throw new UnauthorizedAccessException("User is not authenticated.");

            var hall = await _dbContext.Halls
                .Include(v=>v.Venue)
                .FirstOrDefaultAsync(h=>h.Id==request.HallId,cancellationToken);

            if (hall is null)
                throw new KeyNotFoundException($"Hall with ID '{request.HallId}' was not found.");

            bool isOwner = hall.Venue.OwnerId == currentUserId;
            bool isAdmin = _currentUserServie.IsInRole("Admin");

            if (!isOwner && !isAdmin)
                throw new UnauthorizedAccessException("You are not authorized to create an event in this hall.");

            var newEvent = new Event
            {
               Title=request.Title,
               Description=request.Description,
               EventDate=request.EventDate,
               HallId=request.HallId,
               Tickets=request.TicketTypes.Select(t=>new TicketTier
               {
                   Type=t.Type, 
                   Price=t.Price,
                   TotalQuantity=t.TotalQuantity,
                   AvailableQuantity=t.TotalQuantity
               }).ToList(),
            };

            _dbContext.Events.Add(newEvent);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new ResponseEventDto
            {
                Id = newEvent.Id,
                Title = newEvent.Title,
                Description = newEvent.Description,
                EventDate = newEvent.EventDate,
                HallId = hall.Id,
                HallName = hall.Name,
                VenueName = hall.Venue.Name,
                TicketTypes = newEvent.Tickets.Select(tt => new ResponseTicketTypeDto
                {
                    Id = tt.Id,
                    EventId = newEvent.Id,
                    Type = tt.Type,
                    Price = tt.Price,
                    TotalQuantity = tt.TotalQuantity,
                    AvailableQuantity = tt.AvailableQuantity
                }).ToList()
            };

        }
    }
}
