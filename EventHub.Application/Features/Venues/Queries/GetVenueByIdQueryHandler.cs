using EventHub.Application.Common.InterFaces;
using EventHub.Application.Features.Venues.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Venues.Queries
{ 
    public record GetVenueByIdDto(Guid Id) : IRequest<ResponseVenueDto>;
    public class GetVenueByIdQueryHandler : IRequestHandler<GetVenueByIdDto, ResponseVenueDto>
    {
        private readonly IApplicationDbContext dbContext;
        private readonly IMemoryCache cache;

        public GetVenueByIdQueryHandler(IApplicationDbContext dbContext , IMemoryCache cache)
        {
            this.dbContext = dbContext;
            this.cache = cache;
        }
        public async Task<ResponseVenueDto> Handle(GetVenueByIdDto request, CancellationToken cancellationToken)
        {
            string cacheKey = $"Venue_Detail_{request.Id}";

            if (cache.TryGetValue(cacheKey, out ResponseVenueDto? cachedVenue) && cachedVenue != null)
            { 
                return cachedVenue;
            }

            var venue= await dbContext.Venues
                .AsNoTracking()
                .Where(v=>v.Id==request.Id)
                .Select(v=> new ResponseVenueDto
                    {
                      Id = v.Id,
                      Name = v.Name,
                      Description = v.Description,
                      Address = v.Address,
                      City = v.City,
                      OwnerId = v.OwnerId
                    }).FirstOrDefaultAsync(cancellationToken);

            if (venue is null)
                throw new KeyNotFoundException($"Venue with ID {request.Id} was not found.");

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5))  
                .SetSlidingExpiration(TimeSpan.FromMinutes(1));

            cache.Set(cacheKey, venue, cacheEntryOptions);
            
            return venue;

        }
    }
}
