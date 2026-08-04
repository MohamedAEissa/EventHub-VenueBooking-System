using EventHub.Application.Common.InterFaces;
using EventHub.Application.Features.Venues.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace EventHub.Application.Features.Venues.Commands;

public class UpdateVenueCommandHandler : IRequestHandler<UpdateVenueDto, bool>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMemoryCache cache;

    public UpdateVenueCommandHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService , IMemoryCache cache)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        this.cache = cache;
    }

    public async Task<bool> Handle(UpdateVenueDto request, CancellationToken cancellationToken)
    {
        var venue = await _dbContext.Venues
            .FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken);

        if (venue is null)
            throw new KeyNotFoundException($"Venue with ID '{request.Id}' was not found.");

        
        if (!Guid.TryParse(_currentUserService.UserId, out var currentUserId) || venue.OwnerId != currentUserId)
        {
            throw new UnauthorizedAccessException("You are not authorized to update this venue.");
        }

     
        venue.Name = request.Name;
        venue.Description = request.Description;
        venue.Address = request.Address;
        venue.City = request.City;

        await _dbContext.SaveChangesAsync(cancellationToken);
        cache.Remove("All_Venues_List");
        cache.Remove($"Venue_Detail_{request.Id}");
        return true;
    }
}