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
    public class GetAllVenuesQueryHandler : IRequestHandler<SearchTermDto,List< ResponseVenueDto>>
    {
        private readonly IApplicationDbContext dbContext;
        private readonly IMemoryCache cache;

        public GetAllVenuesQueryHandler(IApplicationDbContext dbContext,IMemoryCache cache)
        {
            this.dbContext = dbContext;
            this.cache = cache;
        }
        public async Task<List<ResponseVenueDto>> Handle(SearchTermDto request, CancellationToken cancellationToken)
        {
            string cacheKey = string.IsNullOrWhiteSpace(request.SearchTerm)
                   ? "All_Venues_List"
                   : $"Venues_Search_{request.SearchTerm.Trim().ToLower()}";

            // -------------------------------------------------------------
            // 👈 5. التشييك الأول: هل الداتا موجودة في الـ RAM؟
            // -------------------------------------------------------------
            if (cache.TryGetValue(cacheKey, out List<ResponseVenueDto>? cachedVenues) && cachedVenues != null)
            {
                // لقينا البيانات متسجلة في الكاش! ارجع بيها فوراً وبدون الوصول للداتابيز
                return cachedVenues;
            }

            // -------------------------------------------------------------
            // 👈 6. لو مش موجودة في الكاش، نطلبها من الداتابيز كالمعتاد
            // -------------------------------------------------------------
            var query = dbContext.Venues.AsNoTracking();

            if (!String.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.Trim().ToLower();
                query = query.Where(v => v.Name.ToLower().Contains(searchTerm));
            }

            var venuesList = await query
                .Select(v => new ResponseVenueDto
                {
                    Id = v.Id,
                    Name = v.Name,
                    Description = v.Description,
                    Address = v.Address,
                    City = v.City,
                    OwnerId = v.OwnerId,
                    OwnerName = v.Owner.FullName,
                })
                .ToListAsync(cancellationToken);

            // -------------------------------------------------------------
            // 👈 7. تحديد مدة صلاحية الكاش
            // -------------------------------------------------------------
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5))  // تنتهي وتتمسح من الـ RAM بعد 5 دقائق
                .SetSlidingExpiration(TimeSpan.FromMinutes(1));   // لو محدش طلبها لمدة دقيقة تتمسح

            // -------------------------------------------------------------
            // 👈 8. حفظ النتيجة في الكاش للمرات القادمة
            // -------------------------------------------------------------
            cache.Set(cacheKey, venuesList, cacheEntryOptions);

            return venuesList;
        }
    }
}
