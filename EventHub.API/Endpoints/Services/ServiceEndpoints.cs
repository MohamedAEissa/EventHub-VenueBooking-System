using EventHub.Application.Features.Services.Commands;
using EventHub.Application.Features.Services.Dtos;
using EventHub.Application.Features.Services.Queries;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace EventHub.API.Endpoints.Services
{
    public static class ServiceEndpoints
    {
        public static void MapServiceEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/venues/{venueId:guid}/services")
                           .WithTags("Services");

            
            group.MapPost("/", async (Guid venueId, CreateServiceDto dto, ISender mediator) =>
            {
                dto.VenueId = venueId;
                var result = await mediator.Send(dto);
                return Results.Created($"/api/venues/{venueId}/services/{result.Id}", result);
            })
            .WithName("CreateService")
            .Produces<ResponseServiceDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();

            
            group.MapGet("/", async (Guid venueId, ISender mediator) =>
            {
                var result = await mediator.Send(new GetServicesByVenueIdQuery(venueId));
                return Results.Ok(result);
            })
            .WithName("GetServicesByVenueId")
            .Produces<IEnumerable<ResponseServiceDto>>(StatusCodes.Status200OK);

            
            group.MapGet("/{id:guid}", async (Guid venueId, Guid id, ISender mediator) =>
            {
                var result = await mediator.Send(new GetServiceByIdQuery(venueId, id));
                return Results.Ok(result);
            })
            .WithName("GetServiceById")
            .Produces<ResponseServiceDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

            
            group.MapPut("/{id:guid}", async (Guid venueId, Guid id, UpdateServiceDto dto, ISender mediator) =>
            {
                dto.VenueId = venueId;
                dto.ServiceId = id;
                var result = await mediator.Send(dto);
                return Results.Ok(result);
            })
            .WithName("UpdateService")
            .Produces<ResponseServiceDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();

            
            group.MapDelete("/{id:guid}", async (Guid venueId, Guid id, ISender mediator) =>
            {
                await mediator.Send(new DeleteServiceCommand(venueId, id));
                return Results.Ok(new { message = "Service Deleted Successfully " });
            })
            .WithName("DeleteService")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();
        }
    }
}