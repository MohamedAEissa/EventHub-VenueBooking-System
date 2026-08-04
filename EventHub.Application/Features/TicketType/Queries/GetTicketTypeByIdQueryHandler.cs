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
    public record GetTicketTypeByIdQuery(Guid Id) :IRequest<ResponseTicketTypeDto>;
    public class GetTicketTypeByIdQueryHandler:IRequestHandler<GetTicketTypeByIdQuery,ResponseTicketTypeDto>
    {
        private readonly IApplicationDbContext _dbContext;

        public GetTicketTypeByIdQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ResponseTicketTypeDto> Handle(GetTicketTypeByIdQuery request, CancellationToken cancellationToken)
        {
            var ticketType = await _dbContext.Tickets.AsNoTracking().FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

            if (ticketType is null)
                throw new KeyNotFoundException($"TicketType with ID '{request.Id}' was not found.");

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
