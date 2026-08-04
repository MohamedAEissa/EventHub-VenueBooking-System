using EventHub.Application.Common.InterFaces;
using EventHub.Application.Features.Tickets.Dtos;
using EventHub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Tickets.Commands
{
    public class PurchaseTicketCommandHandler : IRequestHandler<PurchaseTicketDto, List<ResponseUserTicketDto>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        public PurchaseTicketCommandHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        public async Task<List<ResponseUserTicketDto>> Handle(PurchaseTicketDto request, CancellationToken cancellationToken)
        {
            try
            {

                if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                    throw new UnauthorizedAccessException("User is not authenticated.");


                var userExists = await _dbContext.Users.AnyAsync(u => u.Id == userId, cancellationToken);
                if (!userExists)
                    throw new UnauthorizedAccessException("Authenticated user does not exist in the database.");


                var ticketType = await _dbContext.Tickets
                    .Include(e => e.Event)
                    .FirstOrDefaultAsync(t => t.Id == request.TicketTypeId, cancellationToken);

                if (ticketType is null)
                    throw new KeyNotFoundException($"TicketType with ID '{request.TicketTypeId}' was not found.");


                if (ticketType.AvailableQuantity < request.Quantity)
                    throw new InvalidOperationException($"Not enough tickets available. Remaining tickets: {ticketType.AvailableQuantity}");

                ticketType.AvailableQuantity -= request.Quantity;

                var purchaseDate = DateTime.UtcNow;
                var userTickets = new List<UserTicket>();
                var responseList = new List<ResponseUserTicketDto>();


                for (int i = 0; i < request.Quantity; i++)
                {
                    var ticketId = Guid.NewGuid();
                    var ticketCode = $"EHB-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";

                    var userTicket = new UserTicket
                    {
                        TicketCode = ticketCode,
                        ClientId = userId,
                        TicketId = ticketType.Id,
                        IsUsed = false,
                        PurchaseDate = purchaseDate
                    };

                    userTickets.Add(userTicket);

                    responseList.Add(new ResponseUserTicketDto
                    {
                        TicketId = userTicket.Id,
                        TicketCode = ticketCode,
                        PurchaseDate = purchaseDate,
                        IsUsed = false,
                        EventTitle = ticketType.Event.Title,
                        EventDate = ticketType.Event.EventDate,
                        TicketType = ticketType.Type,
                        PricePaid = ticketType.Price
                    });
                }


                await _dbContext.UserTickets.AddRangeAsync(userTickets, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return responseList;
            }
            catch (DbUpdateException ex)
            {
                
                var innerMessage = ex.InnerException?.Message ?? ex.Message;
                throw new InvalidOperationException($"DATABASE ERROR: {innerMessage}");
            }
        }
    }
}