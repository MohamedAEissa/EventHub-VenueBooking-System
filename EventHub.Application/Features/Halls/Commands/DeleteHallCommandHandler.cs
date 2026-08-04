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
    public class DeleteHallCommandHandler : IRequestHandler<DeleteHallDto, bool>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMemoryCache cache;

        public DeleteHallCommandHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService,IMemoryCache cache)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            this.cache = cache;
        }
        public async Task<bool> Handle(DeleteHallDto request, CancellationToken cancellationToken)
        {
            var venue = await _dbContext.Venues
                 .AsNoTracking()
                 .FirstOrDefaultAsync(v => v.Id == request.VenueId, cancellationToken);

            if (venue is null)
                throw new KeyNotFoundException($"Venue with ID '{request.VenueId}' was not found.");

            if (!Guid.TryParse(_currentUserService.UserId, out var currentUserId) || venue.OwnerId != currentUserId)
                throw new UnauthorizedAccessException("You are not authorized to delete halls from this venue.");

            var hall = await _dbContext.Halls
                .FirstOrDefaultAsync(h => h.Id == request.HallId && h.VenueId == request.VenueId, cancellationToken);

            if (hall is null)
                throw new KeyNotFoundException($"Hall with ID '{request.HallId}' was not found in this venue.");

            
            _dbContext.Halls.Remove(hall);
            await _dbContext.SaveChangesAsync(cancellationToken);
            cache.Remove("All-Hall-List");
            cache.Remove($"Hall-Detail-{request.HallId}");
            return true;
        }
    }
}
