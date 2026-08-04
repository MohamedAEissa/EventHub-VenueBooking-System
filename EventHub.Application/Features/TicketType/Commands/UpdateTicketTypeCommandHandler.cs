using EventHub.Application.Common.InterFaces;
using EventHub.Application.Features.Events.Dtos;
using EventHub.Application.Features.TicketType.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.TicketType.Commands
{
    public class UpdateTicketTypeCommandHandler : IRequestHandler<UpdateTicketTypeDto, ResponseTicketTypeDto>
    {
        private readonly IApplicationDbContext _dbContext;

        public UpdateTicketTypeCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ResponseTicketTypeDto> Handle(UpdateTicketTypeDto request, CancellationToken cancellationToken)
        {
            var ticketType = await _dbContext.Tickets
            .FirstOrDefaultAsync(t => t.Id == request.TicketTypeId, cancellationToken);

            if (ticketType is null)
                throw new KeyNotFoundException($"TicketType with ID '{request.TicketTypeId}' was not found.");

            int soldTicketsCount = ticketType.TotalQuantity - ticketType.AvailableQuantity;

            if (request.TotalQuantity<soldTicketsCount)
                throw new InvalidOperationException(
                $"Cannot set total quantity to {request.TotalQuantity}. Already sold {soldTicketsCount} tickets.");

            ticketType.Type=request.Type;
            ticketType.Price=request.Price;
            ticketType.TotalQuantity=request.TotalQuantity;
            ticketType.AvailableQuantity=request.TotalQuantity-soldTicketsCount;

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
