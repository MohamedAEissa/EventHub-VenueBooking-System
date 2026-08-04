using EventHub.Application.Common.InterFaces;
using EventHub.Application.Features.Services.Dtos;
using EventHub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Services.Commands
{
    public class CreateServiceCommandHandler : IRequestHandler<CreateServiceDto, ResponseServiceDto>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        public CreateServiceCommandHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        public async Task<ResponseServiceDto> Handle(CreateServiceDto request, CancellationToken cancellationToken)
        {
            var venue = await _dbContext.Venues
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == request.VenueId, cancellationToken);

            if (venue is null)
                throw new KeyNotFoundException($"Venue with ID '{request.VenueId}' was not found.");

            if (!Guid.TryParse(_currentUserService.UserId, out var currentUserId) || venue.OwnerId != currentUserId)
                throw new UnauthorizedAccessException("You are not authorized to add services to this venue.");

            
            var service = new Service 
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                IsAvailable = true,
                VenueId = request.VenueId
            };

            _dbContext.Services.Add(service);
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
