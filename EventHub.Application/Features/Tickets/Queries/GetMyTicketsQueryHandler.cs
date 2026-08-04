using EventHub.Application.Common.InterFaces;
using EventHub.Application.Features.Tickets.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Tickets.Queries
{
    public record GetMyTicketsQuery() : IRequest<List<ResponseUserTicketDto>>;
    public class GetMyTicketsQueryHandler : IRequestHandler<GetMyTicketsQuery, List<ResponseUserTicketDto>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        public GetMyTicketsQueryHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        public async Task<List<ResponseUserTicketDto>> Handle(GetMyTicketsQuery request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                throw new UnauthorizedAccessException("User is not authenticated.");

            var userTickets = await _dbContext.UserTickets
                .AsNoTracking()
                .Include(ut => ut.Ticket)
                    .ThenInclude(t => t.Event)
                .Where(ut => ut.ClientId == userId)
                .OrderByDescending(ut => ut.PurchaseDate)
                .Select(ut => new ResponseUserTicketDto
                {
                    TicketId = ut.Id,
                    TicketCode = ut.TicketCode,
                    PurchaseDate = ut.PurchaseDate,
                    IsUsed = ut.IsUsed,
                    EventTitle = ut.Ticket.Event.Title,
                    EventDate = ut.Ticket.Event.EventDate,
                    TicketType = ut.Ticket.Type,
                    PricePaid = ut.Ticket.Price
                })
                .ToListAsync(cancellationToken);

            return userTickets;
        }
    }
}
