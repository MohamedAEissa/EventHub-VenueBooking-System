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
    public record GetVenueBookingsQuery(Guid VenueId) : IRequest<List<ResponseBookingDto>>;

    public class GetVenueBookingsQueryHandler : IRequestHandler<GetVenueBookingsQuery, List<ResponseBookingDto>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        public GetVenueBookingsQueryHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        public async Task<List<ResponseBookingDto>> Handle(GetVenueBookingsQuery request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var currentUserId))
                throw new UnauthorizedAccessException("User is not authenticated.");

            
            var venue = await _dbContext.Venues
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == request.VenueId, cancellationToken);

            if (venue is null)
                throw new KeyNotFoundException($"Venue with ID '{request.VenueId}' was not found.");

            bool isOwner = venue.OwnerId == currentUserId;
            bool isAdmin = _currentUserService.IsInRole("Admin");

            if (!isOwner && !isAdmin)
                throw new UnauthorizedAccessException("You are not authorized to view bookings for this venue.");

           
            var bookings = await _dbContext.Bookings
                .AsNoTracking()
                .Where(b => b.Hall.VenueId == request.VenueId)
                .Include(b => b.Hall)
                    .ThenInclude(h => h.Venue)
                .Include(b => b.BookingServices)
                    .ThenInclude(bs => bs.Service)
                .OrderByDescending(b => b.StartDate)
                .ToListAsync(cancellationToken);

            return bookings.Select(b => new ResponseBookingDto
            {
                Id = b.Id,
                HallId = b.HallId,
                HallName = b.Hall.Name,
                VenueName = b.Hall.Venue.Name,
                StartTime = b.StartDate,
                EndTime = b.EndDate,
                TotalPrice = b.TotalPrice,
                Status = b.Status.ToString(),
                Services = b.BookingServices.Select(bs => new ResponseBookingServicesDto
                {
                    ServiceId = bs.ServiceId,
                    ServiceName = bs.Service.Name,
                    UnitPrice = bs.UnitPrice,
                    Quantity = bs.Quantity
                }).ToList()
            }).ToList();
        }
    }
}
