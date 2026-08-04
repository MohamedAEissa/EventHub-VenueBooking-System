using EventHub.Application.Features.Events.Dtos;
using EventHub.Application.Features.Events.Queries;
using MediatR;

namespace EventHub.API.Endpoints.Events
{
    public static class EventEndpoints
    {
        public static void MapEventsEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/events")
            .WithTags("Events");
 
            group.MapGet("/", async (ISender mediator) =>
            {
                var result = await mediator.Send(new GetAllEventsQuery());
                return Results.Ok(result);
            })
            .WithName("GetAllEvents")
            .Produces<List<ResponseEventDto>>(StatusCodes.Status200OK);

            
            group.MapGet("/{id:guid}", async (Guid id, ISender mediator) =>
            {
                var result = await mediator.Send(new GetEventByIdQuery(id));
                return Results.Ok(result);
            })
            .WithName("GetEventById")
            .Produces<ResponseEventDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

            
            group.MapPost("/", async (CreateEventDto dto, ISender mediator) =>
            {
                var result = await mediator.Send(dto);
                return Results.Created($"/api/events/{result.Id}", result);
            })
            .RequireAuthorization()
            .WithName("CreateEvent")
            .Produces<ResponseEventDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);
        }
    }
}
