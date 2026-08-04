using EventHub.Application.Common.InterFaces;
using EventHub.Application.Features.Events.Dtos;
using EventHub.Application.Features.TicketType.DTOs;
using EventHub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.TicketType.Commands
{
    public class CreateTicketTypeCommandHandler : IRequestHandler<CreateTicketTypeDto, ResponseTicketTypeDto>
    {
        private readonly IApplicationDbContext _dbContext;

        public CreateTicketTypeCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ResponseTicketTypeDto> Handle(CreateTicketTypeDto request, CancellationToken cancellationToken)
        {
            var existEvent = await _dbContext.Events.AnyAsync(e => e.Id == request.EventId, cancellationToken);
            if (!existEvent)
                throw new KeyNotFoundException($"Event with ID '{request.EventId}' was not found.");

            var ticketType = new TicketTier
            {
                EventId = request.EventId,
                Type = request.Type,
                Price = request.Price,
                TotalQuantity = request.TotalQuantity,
                AvailableQuantity = request.TotalQuantity
            };
            _dbContext.Tickets.Add(ticketType);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new ResponseTicketTypeDto
            {
                Id = ticketType.Id,
                EventId = ticketType.EventId,
                Type = ticketType.Type,
                Price = ticketType.Price,
                TotalQuantity = ticketType.TotalQuantity,
                AvailableQuantity = ticketType.AvailableQuantity
            };
        }
    }
}
