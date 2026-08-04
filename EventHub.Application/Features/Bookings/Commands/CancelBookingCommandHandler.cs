using EventHub.Application.Common.InterFaces;
using EventHub.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Bookings.Commands
{
    public record CancelBookingCommand(Guid BookingId) : IRequest<bool>;
    public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, bool>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        public CancelBookingCommandHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }
        public async Task<bool> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var currentUserId))
                throw new UnauthorizedAccessException("User is not authenticated.");

            var booking = await _dbContext.Bookings
            .FirstOrDefaultAsync(b => b.Id == request.BookingId, cancellationToken);

            if (booking is null)
                throw new KeyNotFoundException($"Booking with ID '{request.BookingId}' was not found.");

            if (booking.ClientId != currentUserId)
                throw new UnauthorizedAccessException("You are not authorized to cancel this booking.");

            if (booking.Status == BookingStatus.Cancelled)
                throw new InvalidOperationException("This booking is already cancelled.");

            if (booking.Status == BookingStatus.Completed)
                throw new InvalidOperationException("Cannot cancel a completed booking.");

            if (DateTime.UtcNow >= booking.StartDate)
                throw new InvalidOperationException("Cannot cancel a booking that has already started or passed.");

            booking.Status = BookingStatus.Cancelled;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
