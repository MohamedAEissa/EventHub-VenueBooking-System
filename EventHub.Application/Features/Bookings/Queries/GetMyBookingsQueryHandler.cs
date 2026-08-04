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
    public record GetMyBookingsQuery : IRequest<List<ResponseBookingDto>>;

    public class GetMyBookingsQueryHandler : IRequestHandler<GetMyBookingsQuery, List<ResponseBookingDto>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        public GetMyBookingsQueryHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        public async Task<List<ResponseBookingDto>> Handle(GetMyBookingsQuery request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var currentUserId))
                throw new UnauthorizedAccessException("User is not authenticated.");

            var bookings = await _dbContext.Bookings
                .AsNoTracking()
                .Where(b => b.ClientId == currentUserId)
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
