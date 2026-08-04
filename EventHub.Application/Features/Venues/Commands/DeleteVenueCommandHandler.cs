using EventHub.Application.Common.InterFaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Venues.Commands
{
    public record DeleteVenueDto(Guid Id) : IRequest<bool>;
    public class DeleteVenueCommandHandler : IRequestHandler<DeleteVenueDto, bool>
        {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMemoryCache cache;

        public DeleteVenueCommandHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            this.cache = cache;
        }

        public async Task<bool> Handle(DeleteVenueDto request, CancellationToken cancellationToken)
            {
                var venue = await _dbContext.Venues
                    .FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken);

                if (venue is null)
                    throw new KeyNotFoundException($"Venue with ID '{request.Id}' was not found.");

                if (!Guid.TryParse(_currentUserService.UserId, out var currentUserId) || venue.OwnerId != currentUserId)
                    throw new UnauthorizedAccessException("You are not authorized to delete this venue.");

                _dbContext.Venues.Remove(venue);
                await _dbContext.SaveChangesAsync(cancellationToken);
            cache.Remove("All_Venues_List");
            cache.Remove($"Venue_Detail_{request.Id}");

            return true;
            }
    }
}
