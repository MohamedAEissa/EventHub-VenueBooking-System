using EventHub.Application.Common.InterFaces;
using EventHub.Application.Features.Halls.Dtos;
using EventHub.Domain.Entities;
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
    public class CreateHallCommandHandler : IRequestHandler<CreateHallDto, ResponseHallDto>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMemoryCache cache;

        public CreateHallCommandHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService , IMemoryCache cache)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            this.cache = cache;
        }
        public async Task<ResponseHallDto> Handle(CreateHallDto request, CancellationToken cancellationToken)
        {
            var venue = await _dbContext.Venues
                .FirstOrDefaultAsync(v => v.Id == request.VenueId, cancellationToken);
            if (venue is null)
                throw new KeyNotFoundException($"Venue with ID '{request.VenueId}' was not found.");

            if (!Guid.TryParse(_currentUserService.UserId, out var currentUserId) || venue.OwnerId != currentUserId)
                throw new UnauthorizedAccessException("You are not authorized to add a hall to this venue.");

            var hall = new Hall
            {
                
                VenueId = request.VenueId,
                Name = request.Name,
                Capacity = request.Capacity,
                PricePerHour = request.PricePerHour,
                IsActive = true
            };

            _dbContext.Halls.Add(hall);
            await _dbContext.SaveChangesAsync(cancellationToken);

            cache.Remove("All-Hall-List");
            cache.Remove($"Hall-Detail-{hall.Id}");
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
