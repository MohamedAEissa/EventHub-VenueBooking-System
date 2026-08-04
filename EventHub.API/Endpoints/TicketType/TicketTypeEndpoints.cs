using EventHub.Application.Features.Events.Dtos;
using EventHub.Application.Features.TicketType.DTOs;
using EventHub.Application.Features.TicketType.Queries;
using MediatR;

namespace EventHub.API.Endpoints.TicketType
{
    public static class TicketTypeEndpoints
    {
        public static void MapTicketTypeEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/ticket-types")
                           .WithTags("Ticket Types")
                           .RequireAuthorization();

            
            group.MapPost("/", async (CreateTicketTypeDto dto, ISender mediator) =>
            {
                var result = await mediator.Send(dto);
                return Results.Created($"/api/ticket-types/{result.Id}", result);
            })
            .WithName("CreateTicketType")
            .Produces<ResponseTicketTypeDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound);

            
            group.MapPut("/{id:guid}", async (UpdateTicketTypeDto dto, ISender mediator) =>
            {
                var result = await mediator.Send(dto);
                return Results.Ok(result);
            })
            .WithName("UpdateTicketType")
            .Produces<ResponseTicketTypeDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound);

            
            group.MapGet("/{id:guid}", async (Guid id, ISender mediator) =>
            {
                var result = await mediator.Send(new GetTicketTypeByIdQuery(id)); 
                return Results.Ok(result);
            })
            .WithName("GetTicketTypeById")
            .Produces<ResponseTicketTypeDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

           
            group.MapGet("/event/{eventId:guid}", async (Guid eventId, ISender mediator) =>
            {
                var result = await mediator.Send(new GetTicketTypesByEventIdQuery(eventId));
                return Results.Ok(result);
            })
            .WithName("GetTicketTypesByEventId")
            .Produces<List<ResponseTicketTypeDto>>(StatusCodes.Status200OK);
        }
    }
}
