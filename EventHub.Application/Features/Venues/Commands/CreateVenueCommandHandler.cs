using EventHub.Application.Common.InterFaces;
using EventHub.Application.Features.Venues.Dtos;
using EventHub.Domain.Entities;
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
    public class CreateVenueCommandHandler : IRequestHandler<CreateVenueDto, ResponseVenueDto>
    {
        private readonly IApplicationDbContext dbContext;
        private readonly ICurrentUserService currentUserService;
        private readonly IMemoryCache cache;

        public CreateVenueCommandHandler(IApplicationDbContext dbContext , ICurrentUserService currentUserService, IMemoryCache cache)
        {
            this.dbContext = dbContext;
            this.currentUserService = currentUserService;
            this.cache = cache;
        }
        public async Task<ResponseVenueDto> Handle(CreateVenueDto request, CancellationToken cancellationToken)
        {
            var currentUserIdAsString = currentUserService.UserId;
            if (string.IsNullOrWhiteSpace(currentUserIdAsString))
                throw new UnauthorizedAccessException("User is not authenticated");

            var currentUserId = Guid.Parse(currentUserIdAsString);

            var owner =await dbContext.Users.FirstOrDefaultAsync(u => u.Id == currentUserId,cancellationToken);
            if (owner==null)
                throw new Exception("Owner user not found");

            var venue = new Venue
            {
                Name = request.Name,
                Description = request.Description,
                Address = request.Address,
                City = request.City,
                OwnerId = owner.Id,
            };

            dbContext.Venues.Add(venue);
            await dbContext.SaveChangesAsync(cancellationToken);
            cache.Remove("All_Venues_List");
            cache.Remove($"Venue_Detail_{venue.Id}");

            return new ResponseVenueDto
            {
                Id=venue.Id,
                Name = venue.Name,
                Description = venue.Description,
                Address = venue.Address,
                City = venue.City,
                OwnerId = owner.Id,
                OwnerName = owner.FullName
            };
        }
    }
}
