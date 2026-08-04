using EventHub.Application.Features.Reviews.Dtos;
using EventHub.Application.Features.Reviews.Queries;
using MediatR;

namespace EventHub.API.Endpoints.Review
{
    public static class ReviewEndpoints
    {
        public static void MapReviewEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/reviews")
                           .WithTags("Reviews");

            
            group.MapPost("/venue", async (CreateReviewDto dto, ISender mediator) =>
            {
                var reviewId = await mediator.Send(dto);
                return Results.Created($"/api/reviews/{reviewId}", new { Id = reviewId });
            })
            .RequireAuthorization()
            .WithName("CreateVenueReview")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

           
            group.MapGet("/venue/{venueId:guid}", async (Guid venueId, ISender mediator) =>
            {
                var result = await mediator.Send(new GetVenueReviewsQuery(venueId));
                return Results.Ok(result);
            })
            .WithName("GetVenueReviews")
            .Produces<List<ResponseReviewDto>>(StatusCodes.Status200OK);
        }
    }
}
