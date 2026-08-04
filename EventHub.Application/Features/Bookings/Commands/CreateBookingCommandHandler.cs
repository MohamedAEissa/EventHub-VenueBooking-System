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
using System.Threading;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Bookings.Commands
{
    public class CreateBookingCommandHandler : IRequestHandler<CreateBookingDto, ResponseBookingDto>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        public CreateBookingCommandHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }
        public async Task<ResponseBookingDto> Handle(CreateBookingDto request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var clientId))
                throw new UnauthorizedAccessException("User is not authenticated.");

            if (request.EndTime <= request.StartTime)
                throw new ArgumentException("EndDate must be after StartDate.");

            var hall = await _dbContext.Halls
                .Include(h=>h.Venue)
                .FirstOrDefaultAsync(h=>h.Id==request.HallId,cancellationToken);

            if (hall is null)
                throw new KeyNotFoundException($"Hall with ID '{request.HallId}' was not found.");


            var isOverLopped = await _dbContext.Bookings.AnyAsync(b =>
                    
                b.HallId == request.HallId && 
                b.Status!=BookingStatus.Cancelled &&
                request.EndTime < b.StartDate && 
                request.StartTime > b.EndDate,
                cancellationToken
                
            );

            if (isOverLopped)
                throw new InvalidOperationException("The hall is already booked for the selected time range.");

            var totalHours = (decimal)(request.EndTime - request.StartTime).TotalHours;
            decimal bookingTotalPrice = totalHours* hall.PricePerHour;

            var bookingServicesList = new List<BookingService>();
            var responseServicesList = new List<ResponseBookingServicesDto>();

            if (request.Services.Any())
            {
                var requestedServiceIds = request.Services.Select(s=>s.ServiceId).ToList();

                var Services = await _dbContext.Services.
                    Where(s=> requestedServiceIds.Contains(s.Id) &&
                    s.VenueId==hall.VenueId&&s.IsAvailable)
                    .ToListAsync(cancellationToken);

                foreach (var  userInputService in request.Services)
                {
                    var Service = Services.FirstOrDefault(s => s.Id == userInputService.ServiceId);

                    if (Service is null)
                        throw new KeyNotFoundException($"Service with ID '{userInputService.ServiceId}' is invalid or not available.");

                    var bookingService = new BookingService
                    {
                        ServiceId=Service.Id,
                        Quantity=userInputService.Quantity,
                        UnitPrice=Service.Price
                    };
                    
                    bookingServicesList.Add(bookingService);

                    bookingTotalPrice += (Service.Price * userInputService.Quantity);

                    responseServicesList.Add(new ResponseBookingServicesDto
                    {
                        ServiceId=Service.Id,
                        ServiceName=Service.Name,
                        UnitPrice=Service.Price,
                        Quantity=userInputService.Quantity    

                    });
                }
            }

            var booking = new Booking
            {
                ClientId = clientId,
                HallId = request.HallId,
                StartDate = request.StartTime,
                EndDate = request.EndTime,
                TotalPrice = bookingTotalPrice,
                Status = BookingStatus.Pending, 
                BookingServices = bookingServicesList
            };
            _dbContext.Bookings.Add(booking);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new ResponseBookingDto
            {
                Id = booking.Id,
                HallId = hall.Id,
                HallName = hall.Name,
                VenueName = hall.Venue.Name,
                StartTime = booking.StartDate,
                EndTime = booking.EndDate,
                TotalPrice = booking.TotalPrice,
                Status = booking.Status.ToString(),
                Services = responseServicesList
            };

        }
    }
}
