using EventHub.Application.Common.InterFaces;
using EventHub.Application.Features.Reviews.Dtos;
using EventHub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Reviews.Commands
{
    public class CreateVenueReviewCommandHandler : IRequestHandler<CreateReviewDto, ResponseReviewDto>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        public CreateVenueReviewCommandHandler(IApplicationDbContext dbContext,ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }
        public async Task<ResponseReviewDto> Handle(CreateReviewDto request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_currentUserService.UserId, out var userId))
                throw new UnauthorizedAccessException("User is not authenticated.");
            
            var user = await _dbContext.Users.FirstOrDefaultAsync(u=>u.Id==userId,cancellationToken);

            if (request.Rate<1 || request.Rate>5)
                throw new ArgumentException("Rating must be between 1 and 5.");

            var venueExist = await _dbContext.Venues.AsNoTracking().AnyAsync(v => v.Id == request.VenueId,cancellationToken);
            if (!venueExist)
                throw new KeyNotFoundException($"Venue with ID '{request.VenueId}' was not found.");

            var hasVisitedVenue = await _dbContext.UserTickets
                .AsNoTracking()
                .AnyAsync(t=>t.ClientId == userId &&
                t.Ticket.Event.Hall.Venue.Id==request.VenueId,cancellationToken);

            if (!hasVisitedVenue)
                throw new InvalidOperationException("You can only review venues where you have attended an event.");

            var alreadyReviewed = await _dbContext.Reviews
            .AsNoTracking()
            .AnyAsync(r => r.ClientId == userId && r.VenueId == request.VenueId, cancellationToken);
           
            if (alreadyReviewed)
                throw new InvalidOperationException("You have already submitted a review for this venue.");

            var review = new Review
            {
                ClientId = userId,
                VenueId = request.VenueId,
                Rate = request.Rate,
                Comment = request.Comment
            };
            _dbContext.Reviews.Add(review);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new ResponseReviewDto
            {
                Id=review.Id,
                FullName=user.FullName,
                VenueId=review.VenueId,
                Rate=review.Rate,
                Comment=review.Comment,
                CreatedAt=DateTime.UtcNow, 
            };

        }
    }
}
