using EventHub.Application.Common.InterFaces;
using EventHub.Application.Features.Services.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Services.Queries
{
    public record GetServicesByVenueIdQuery(Guid VenueId) : IRequest<IEnumerable<ResponseServiceDto>>;
    public class GetServicesByVenueIdQueryHandler : IRequestHandler<GetServicesByVenueIdQuery, IEnumerable<ResponseServiceDto>>
    {
        private readonly IApplicationDbContext _dbContext;

        public GetServicesByVenueIdQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<ResponseServiceDto>> Handle(GetServicesByVenueIdQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Services
                  .AsNoTracking()
                  .Where(s => s.VenueId == request.VenueId)
                  .Select(s => new ResponseServiceDto
                  {
                      Id = s.Id,
                      Name = s.Name,
                      Description = s.Description,
                      Price = s.Price,
                      IsAvailable = s.IsAvailable,
                      VenueId = s.VenueId
                  })
                  .ToListAsync(cancellationToken);
        }
    }
}
