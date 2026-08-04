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
    public class GetAllHallsByVenueIdQueryHandler : IRequestHandler<GetAllHallsByVenueIdDto, IEnumerable<ResponseHallDto>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IMemoryCache _cache;

        public GetAllHallsByVenueIdQueryHandler(IApplicationDbContext dbContext,IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }
        public async Task<IEnumerable<ResponseHallDto>> Handle(GetAllHallsByVenueIdDto request, CancellationToken cancellationToken)
        {
            var cachKey = "All-Hall-List";

            if (_cache.TryGetValue(cachKey,out IEnumerable<ResponseHallDto>? cacheHall) && cacheHall!=null)
            {
                return cacheHall;
            }
            var halls = await _dbContext.Halls
                .AsNoTracking()
                .Where(h => h.VenueId == request.VenueId)
                .Select(h => new ResponseHallDto
                {
                    Id = h.Id,
                    Name = h.Name,
                    Capacity = h.Capacity,
                    PricePerHour = h.PricePerHour,
                    IsActive = h.IsActive
                })
                .ToListAsync(cancellationToken);
             var cacheEntriyOption = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
                .SetSlidingExpiration(TimeSpan.FromMinutes(1));

            _cache.Set(cachKey,halls,cacheEntriyOption);
            return halls;
        }
    }
}
