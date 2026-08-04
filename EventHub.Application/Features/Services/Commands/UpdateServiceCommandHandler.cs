using EventHub.Application.Common.InterFaces;
using EventHub.Application.Features.Services.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Services.Commands
{
    public class UpdateServiceCommandHandler : IRequestHandler<UpdateServiceDto, ResponseServiceDto>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        public UpdateServiceCommandHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }
        public async Task<ResponseServiceDto> Handle(UpdateServiceDto request, CancellationToken cancellationToken)
        {
            var venue = await _dbContext.Venues
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == request.VenueId, cancellationToken);

            if (venue is null)
                throw new KeyNotFoundException($"Venue with ID '{request.VenueId}' was not found.");

            if (!Guid.TryParse(_currentUserService.UserId, out var currentUserId) || venue.OwnerId != currentUserId)
                throw new UnauthorizedAccessException("You are not authorized to update services in this venue.");

           
            var service = await _dbContext.Services
                .FirstOrDefaultAsync(s => s.Id == request.ServiceId && s.VenueId == request.VenueId, cancellationToken);

            if (service is null)
                throw new KeyNotFoundException($"Service with ID '{request.ServiceId}' was not found in this venue.");

          
            service.Name = request.Name;
            service.Description = request.Description;
            service.Price = request.Price;
            service.IsAvailable = request.IsAvailable;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new ResponseServiceDto
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                Price = service.Price,
                IsAvailable = service.IsAvailable,
                VenueId = service.VenueId
            };
        }
    }
}
