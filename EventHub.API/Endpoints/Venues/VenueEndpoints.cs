using EventHub.Application.Features.Venues.Commands;
using EventHub.Application.Features.Venues.Dtos;
using EventHub.Application.Features.Venues.Queries;
using MediatR;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace EventHub.API.Endpoints.Venues
{
 public static class VenueEndpoints
    {
        public static void MapVenueEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/venues")
                           .WithTags("Venues");

            group.MapPost("/", async (CreateVenueDto dto, ISender mediator) =>
            {
                var result = await mediator.Send(dto);
                return Results.Created($"/api/venues/{result.Id}", result);
            })
            .WithName("CreateVenue")
            .Produces<ResponseVenueDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();

            group.MapGet("/{id:guid}", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new GetVenueByIdDto(id));
                return Results.Ok(result);
            });

            group.MapGet("/", async ([AsParameters] SearchTermDto dto, ISender mediator) => 
            {
                var result = await mediator.Send(dto);
                return Results.Ok(result);

            });

            group.MapPut("/{id:guid}", async (Guid id, UpdateVenueDto dto, ISender mediator) =>
            {
                dto.Id = id;
                await mediator.Send(dto);
                return Results.Ok(new {message= "Venue updated successfully." }); 
            })
            .WithName("UpdateVenue")
            .Produces(StatusCodes.Status204NoContent)
            .RequireAuthorization();

            group.MapDelete("/{id:guid}", async (Guid id, ISender mediator) =>
            {
                await mediator.Send(new DeleteVenueDto(id));
                return Results.Ok(new { message = "Venue deleted successfully." });
            })
            .WithName("DeleteVenue")
            .Produces(StatusCodes.Status204NoContent)
            .RequireAuthorization();
        }
    }
}
