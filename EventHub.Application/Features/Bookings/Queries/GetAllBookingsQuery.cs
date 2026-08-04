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
    public record GetAllBookingsDto : IRequest<List<ResponseBookingDto>>;
    public class GetAllBookingsQueryHandler : IRequestHandler<GetAllBookingsDto, List<ResponseBookingDto>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        public GetAllBookingsQueryHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        public async Task<List<ResponseBookingDto>> Handle(GetAllBookingsDto request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsInRole("Admin"))
                throw new UnauthorizedAccessException("Only Admins are allowed to retrieve all system bookings.");

            var bookings = await _dbContext.Bookings
                .AsNoTracking()
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
                EndTime= b.EndDate,
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
