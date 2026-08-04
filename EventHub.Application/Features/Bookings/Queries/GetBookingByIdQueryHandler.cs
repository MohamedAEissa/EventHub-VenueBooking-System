using EventHub.Application.Common.InterFaces;
using EventHub.Application.Features.Bookings.Dtos;
using EventHub.Application.Features.Services.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Bookings.Queries
{
    public record GetBookingByIdDto(Guid BookingId) : IRequest<ResponseBookingDto>;
    public class GetBookingByIdQueryHandler : IRequestHandler<GetBookingByIdDto, ResponseBookingDto>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        public GetBookingByIdQueryHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }
        public async Task<ResponseBookingDto> Handle(GetBookingByIdDto request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var currentUserId))
                throw new UnauthorizedAccessException("User is not authenticated.");

            
            var booking = await _dbContext.Bookings
                .AsNoTracking()
                .Include(b => b.Hall)
                    .ThenInclude(h => h.Venue)
                .Include(b => b.BookingServices)
                    .ThenInclude(bs => bs.Service)
                .FirstOrDefaultAsync(b => b.Id == request.BookingId, cancellationToken);

            if (booking is null)
                throw new KeyNotFoundException($"Booking with ID '{request.BookingId}' was not found.");

            
            bool isClient = booking.ClientId == currentUserId;
            bool isVenueOwner = booking.Hall.Venue.OwnerId == currentUserId;
            bool isAdmin = _currentUserService.IsInRole("Admin");

            if (!isClient && !isVenueOwner && !isAdmin)
                throw new UnauthorizedAccessException("You do not have permission to view this booking.");

           
            return new ResponseBookingDto
            {
                Id = booking.Id,
                HallId = booking.HallId,
                HallName = booking.Hall.Name,
                VenueName = booking.Hall.Venue.Name,
                StartTime = booking.StartDate,
                EndTime = booking.EndDate,
                TotalPrice = booking.TotalPrice,
                Status = booking.Status.ToString(),
                Services = booking.BookingServices.Select(bs => new ResponseBookingServicesDto
                {
                    ServiceId = bs.ServiceId,
                    ServiceName = bs.Service.Name,
                    UnitPrice = bs.UnitPrice,
                    Quantity = bs.Quantity
                }).ToList()
            };
        }
    }
}
