using EventHub.Application.Common.InterFaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Services.Commands
{
    public record DeleteServiceCommand(Guid VenueId, Guid ServiceId) : IRequest<bool>;
    public class DeleteServiceCommandHandler : IRequestHandler<DeleteServiceCommand, bool>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        public DeleteServiceCommandHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(DeleteServiceCommand request, CancellationToken cancellationToken)
        {
            var venue = await _dbContext.Venues
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == request.VenueId, cancellationToken);

            if (venue is null)
                throw new KeyNotFoundException($"Venue with ID '{request.VenueId}' was not found.");

            if (!Guid.TryParse(_currentUserService.UserId, out var currentUserId) || venue.OwnerId != currentUserId)
                throw new UnauthorizedAccessException("You are not authorized to delete services from this venue.");

            var service = await _dbContext.Services
                .FirstOrDefaultAsync(s => s.Id == request.ServiceId && s.VenueId == request.VenueId, cancellationToken);

            if (service is null)
                throw new KeyNotFoundException($"Service with ID '{request.ServiceId}' was not found in this venue.");

            _dbContext.Services.Remove(service);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
