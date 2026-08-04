using EventHub.Application.Common.InterFaces;
using EventHub.Application.Features.Bookings.Dtos;
using EventHub.Application.Features.Services.Dtos;
using EventHub.Domain.Entities;
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
    public class UpdateBookingCommandHandler : IRequestHandler<UpdateBookingDto, ResponseBookingDto>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        public UpdateBookingCommandHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;

        }

        public async Task<ResponseBookingDto> Handle(UpdateBookingDto request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var currentUserId))
                throw new UnauthorizedAccessException("User is not authenticated.");

            if (request.EndDate <= request.StartDate)
                throw new ArgumentException("EndDate must be after StartDate.");

            
            var booking = await _dbContext.Bookings
                .Include(b => b.Hall)
                    .ThenInclude(h => h.Venue)
                .Include(b => b.BookingServices)
                .FirstOrDefaultAsync(b => b.Id == request.BookingId, cancellationToken);

            if (booking is null)
                throw new KeyNotFoundException($"Booking with ID '{request.BookingId}' was not found.");

            if (booking.ClientId != currentUserId)
                throw new UnauthorizedAccessException("You are not authorized to update this booking.");

            if (booking.Status == BookingStatus.Cancelled || booking.Status == BookingStatus.Completed)
                throw new InvalidOperationException($"Cannot update a booking that is {booking.Status}.");

            
            var isOverlapped = await _dbContext.Bookings.AnyAsync(b =>
                b.Id != request.BookingId && 
                b.HallId == booking.HallId &&
                b.Status != BookingStatus.Cancelled &&
                request.StartDate < b.EndDate &&
                request.EndDate > b.StartDate,
                cancellationToken);

            if (isOverlapped)
                throw new InvalidOperationException("The hall is already booked for the new selected time range.");

            
            var totalHours = (decimal)(request.EndDate - request.StartDate).TotalHours;
            decimal newTotalPrice = totalHours * booking.Hall.PricePerHour;

          
            _dbContext.BookingServices.RemoveRange(booking.BookingServices);
            booking.BookingServices.Clear();

            var responseServicesList = new List<ResponseBookingServicesDto>();

            if (request.Services.Any())
            {
                var requestedServiceIds = request.Services.Select(s => s.ServiceId).ToList();

                var Services = await _dbContext.Services
                    .Where(s => requestedServiceIds.Contains(s.Id) && s.VenueId == booking.Hall.VenueId && s.IsAvailable)
                    .ToListAsync(cancellationToken);

                foreach (var userinputService in request.Services)
                {
                    var Service = Services.FirstOrDefault(s => s.Id == userinputService.ServiceId);
                    if (Service is null)
                        throw new KeyNotFoundException($"Service with ID '{userinputService.ServiceId}' is invalid or not available.");

                    var newBookingService = new BookingService
                    {
                        BookingId = booking.Id,
                        ServiceId = Service.Id,
                        Quantity = userinputService.Quantity,
                        UnitPrice = Service.Price
                    };

                    booking.BookingServices.Add(newBookingService);
                    newTotalPrice += (Service.Price * userinputService.Quantity);

                    responseServicesList.Add(new ResponseBookingServicesDto
                    {
                        ServiceId = Service.Id,
                        ServiceName = Service.Name,
                        UnitPrice = Service.Price,
                        Quantity = userinputService.Quantity
                    });
                }
            }

           
            booking.StartDate = request.StartDate;
            booking.EndDate = request.EndDate;
            booking.TotalPrice = newTotalPrice;

            await _dbContext.SaveChangesAsync(cancellationToken);

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
                Services = responseServicesList
            };
        
    }
    }  
}