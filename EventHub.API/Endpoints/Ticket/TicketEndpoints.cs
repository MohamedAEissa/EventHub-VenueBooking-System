using EventHub.Application.Features.Tickets.Dtos;
using EventHub.Application.Features.Tickets.Queries;
using MediatR;

namespace EventHub.API.Endpoints.Ticket
{
    public static class TicketEndpoints
    {
        public static void MapTicketEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/tickets")
                           .WithTags("Tickets")
                           .RequireAuthorization();

            
            group.MapPost("/purchase", async (PurchaseTicketDto dto, ISender mediator) =>
            {
                var result = await mediator.Send(dto);
                return Results.Ok(result);
            })
            .WithName("PurchaseTicket")
            .Produces<List<ResponseUserTicketDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound);

            group.MapGet("/my-tickets", async (ISender mediator) =>
            {
                var result = await mediator.Send(new GetMyTicketsQuery());
                return Results.Ok(result);
            })
             .WithName("GetMyTickets")
             .Produces<List<ResponseUserTicketDto>>(StatusCodes.Status200OK)
             .Produces(StatusCodes.Status401Unauthorized);

        }
    }
}
