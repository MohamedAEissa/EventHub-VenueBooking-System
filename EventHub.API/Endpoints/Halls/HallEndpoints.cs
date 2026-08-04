using EventHub.Application.Features.Halls.Dtos;
using MediatR;

namespace EventHub.API.Endpoints.Halls
{
    public static class HallEndpoints
    {
        public static void MapHallEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/venues/{venueId:guid}/halls")
                           .WithTags("Halls");

            // Create Hall
            group.MapPost("/", async (Guid venueId, CreateHallDto dto, ISender mediator) =>
            {
                dto.VenueId = venueId; 
                var result = await mediator.Send(dto);
                return Results.Created($"/api/venues/{venueId}/halls/{result.Id}", result);
            })
            .WithName("CreateHall")
            .Produces<ResponseHallDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();

            group.MapGet("/{id:guid}", async (Guid venueId, Guid id, ISender mediator) =>
            {
                var result = await mediator.Send(new GetHallByIdDto(venueId, id));
                return Results.Ok(result);
            })
             .WithName("GetHallById")
             .Produces<ResponseHallDto>(StatusCodes.Status200OK)
             .Produces(StatusCodes.Status404NotFound);

            group.MapGet("/", async (Guid venueId, ISender mediator) =>
            {
                var result = await mediator.Send(new GetAllHallsByVenueIdDto(venueId));
                return Results.Ok(result);
            })
            .WithName("GetAllHallsByVenueId")
            .Produces<IEnumerable<ResponseHallDto>>(StatusCodes.Status200OK);

            group.MapPut("/{id:guid}", async (Guid venueId, Guid id, UpdateHallDto dto, ISender mediator) =>
            {
                dto.VenueId = venueId;
                dto.HallId = id;

                var result = await mediator.Send(dto);
                return Results.Ok(result);
            })
            .WithName("UpdateHall")
            .Produces<ResponseHallDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();

            group.MapDelete("/{id:guid}", async (Guid venueId, Guid id, ISender mediator) =>
            {
                await mediator.Send(new DeleteHallDto(venueId, id));
                return Results.Ok(new {message="Hall Deleted Successfully "});
            })
            .WithName("DeleteHall")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();

        }
    }
}
