using EventHub.Application.Common.InterFaces;
using EventHub.Application.Features.Reviews.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Reviews.Queries
{
    public record GetVenueReviewsQuery(Guid VenueId) : IRequest<List<ResponseReviewDto>>;

    public class GetVenueReviewsQueryHandler : IRequestHandler<GetVenueReviewsQuery, List<ResponseReviewDto>>
    {
        private readonly IApplicationDbContext _dbContext;

        public GetVenueReviewsQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ResponseReviewDto>> Handle(GetVenueReviewsQuery request, CancellationToken cancellationToken)
        {
            var reviews = await _dbContext.Reviews
            .AsNoTracking()
            .Include(r => r.Client)
            .Where(r => r.VenueId == request.VenueId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ResponseReviewDto
            {
                Id = r.Id,
                VenueId = r.VenueId,
                FullName = r.Client.FullName,
                Rate = r.Rate,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync(cancellationToken);

            return reviews;
        }
    }
}
