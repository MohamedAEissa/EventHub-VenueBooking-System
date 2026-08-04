using EventHub.Application.Common.InterFaces;
using EventHub.Application.Features.Bookings.Dtos;
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
    public class ChangeBookingStatusCommandHandler : IRequestHandler<ChangeBookingStatusDto, bool>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        public ChangeBookingStatusCommandHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(ChangeBookingStatusDto request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var currentUserId))
                throw new UnauthorizedAccessException("User is not authenticated.");

            
            var booking = await _dbContext.Bookings
                .Include(b => b.Hall)
                    .ThenInclude(h => h.Venue)
                .FirstOrDefaultAsync(b => b.Id == request.BookingId, cancellationToken);

            if (booking is null)
                throw new KeyNotFoundException($"Booking with ID '{request.BookingId}' was not found.");

            bool isOwner = booking.Hall.Venue.OwnerId == currentUserId;
            bool isAdmin = _currentUserService.IsInRole("Admin");

            if (!isOwner && !isAdmin)
                throw new UnauthorizedAccessException("You are not authorized to change the status of this booking.");

           
            if (booking.Status == request.NewStatus)
                return true;

            if (booking.Status == BookingStatus.Cancelled || booking.Status == BookingStatus.Completed)
                throw new InvalidOperationException($"Cannot change status of a booking that is already {booking.Status}.");

            if (request.NewStatus == BookingStatus.Completed && booking.EndDate > DateTime.UtcNow)
                throw new InvalidOperationException("Cannot set status to Completed before the booking end time.");

            booking.Status = request.NewStatus;
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
