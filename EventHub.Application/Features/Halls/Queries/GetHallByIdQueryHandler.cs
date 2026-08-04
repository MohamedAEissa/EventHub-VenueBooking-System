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

namespace EventHub.Application.Features.Halls.Queries
{
    public class GetHallByIdQueryHandler : IRequestHandler<GetHallByIdDto, ResponseHallDto>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IMemoryCache cache;

        public GetHallByIdQueryHandler(IApplicationDbContext dbContext , IMemoryCache cache)
        {
            _dbContext = dbContext;
            this.cache = cache;
        }
        public async Task<ResponseHallDto> Handle(GetHallByIdDto request, CancellationToken cancellationToken)
        {
            var cahceKey = $"Hall-Detail-{request.HallId}";

            if (cache.TryGetValue(cahceKey,out ResponseHallDto? cachedHall )&& cachedHall!=null)
            {
                return cachedHall;
            }

            var hall=await _dbContext.Halls
                .AsNoTracking()
                .Where(h=>h.VenueId==request.VenueId&& h.Id==request.HallId)
                .Select(h=>new ResponseHallDto{
                Id= h.Id,
                Name = h.Name,
                Capacity = h.Capacity,
                PricePerHour = h.PricePerHour,
                IsActive = h.IsActive
            }).FirstOrDefaultAsync(cancellationToken);

            if (hall is null)
                throw new KeyNotFoundException($"Hall with ID '{request.HallId}' was not found in this venue.");

            var cacheEntiryOption = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
                .SetSlidingExpiration(TimeSpan.FromMinutes(1));

            cache.Set(cahceKey, hall, cacheEntiryOption);

            return hall;
        }
    }
}
