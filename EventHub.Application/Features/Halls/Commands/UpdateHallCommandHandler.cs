using EventHub.Application.Common.InterFaces;
using EventHub.Application.Features.Halls.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Halls.Commands
{
    public class UpdateHallCommandHandler : IRequestHandler<UpdateHallDto, ResponseHallDto>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMemoryCache cache;

        public UpdateHallCommandHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService,IMemoryCache cache)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            this.cache = cache;
        }
        public async Task<ResponseHallDto> Handle(UpdateHallDto request, CancellationToken cancellationToken)
        {
            var venue = await _dbContext.Venues
                 .AsNoTracking()
                 .FirstOrDefaultAsync(v => v.Id == request.VenueId, cancellationToken);

            if (venue is null)
                throw new KeyNotFoundException($"Venue with ID '{request.VenueId}' was not found.");

            if(!Guid.TryParse(_currentUserService.UserId, out var currentUserId) || venue.OwnerId != currentUserId)
                throw new UnauthorizedAccessException("You are not authorized to update halls in this venue.");

            var hall = await _dbContext.Halls
                .FirstOrDefaultAsync(h => h.Id == request.HallId && h.VenueId == request.VenueId, cancellationToken);

            if (hall is null)
                throw new KeyNotFoundException($"Hall with ID '{request.HallId}' was not found in this venue.");

            hall.Name = request.Name;
            hall.Capacity = request.Capacity;
            hall.PricePerHour = request.PricePerHour;

            await _dbContext.SaveChangesAsync(cancellationToken);
            cache.Remove("All-Hall-List");
            cache.Remove($"Hall-Detail-{request.HallId}");
            return new ResponseHallDto
            {
                Id = hall.Id,
                Name = hall.Name,
                Capacity = hall.Capacity,
                PricePerHour = hall.PricePerHour,
                IsActive = hall.IsActive
            };
        }
    }
}
