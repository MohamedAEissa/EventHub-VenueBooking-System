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
    public record GetServiceByIdQuery(Guid VenueId, Guid ServiceId) : IRequest<ResponseServiceDto>;
    public class GetServiceByIdQueryHandler : IRequestHandler<GetServiceByIdQuery, ResponseServiceDto>
    {
        private readonly IApplicationDbContext _dbContext;

        public GetServiceByIdQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ResponseServiceDto> Handle(GetServiceByIdQuery request, CancellationToken cancellationToken)
        {
            var service = await _dbContext.Services
                .AsNoTracking()
                .Where(s => s.VenueId == request.VenueId && s.Id == request.ServiceId)
                .Select(s => new ResponseServiceDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    Price = s.Price,
                    IsAvailable = s.IsAvailable,
                    VenueId = s.VenueId
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (service is null)
                throw new KeyNotFoundException($"Service with ID '{request.ServiceId}' was not found in this venue.");

            return service;
        }
    }
}
