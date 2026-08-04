using EventHub.Application.Features.Bookings.Commands;
using EventHub.Application.Features.Bookings.Dtos;
using EventHub.Application.Features.Bookings.Queries;
using MediatR;

namespace EventHub.API.Endpoints.BookingEndpoints
{
    public static class BookingEndpoints
    {
        public static void MapBookingEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/bookings")
                           .WithTags("Bookings")
                           .RequireAuthorization();

            group.MapPost("/", async (CreateBookingDto dto, ISender mediator) =>
            {
                var result = await mediator.Send(dto);
                return Results.Created($"/api/bookings/{result.Id}", result);
            })
            .WithName("CreateBooking")
            .Produces<ResponseBookingDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);


            group.MapPatch("/{id:guid}/cancel", async (Guid id, ISender mediator) =>
            {
                await mediator.Send(new CancelBookingCommand(id));
                return Results.Ok(new { Message = "Booking cancelled successfully." });
            })
            .WithName("CancelBooking")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound);


            group.MapPut("/{id:guid}", async (Guid id, UpdateBookingDto dto, ISender mediator) =>
            {
                dto.BookingId = id;
                var result = await mediator.Send(dto);
                return Results.Ok(result);
            })
            .WithName("UpdateBooking")
            .Produces<ResponseBookingDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();



            group.MapPatch("/{id:guid}/status", async (Guid id, ChangeBookingStatusDto dto, ISender mediator) =>
            {
                dto.BookingId = id;
                var result = await mediator.Send(dto);
                return Results.Ok(new { message = "Booking status updated successfully." });
            })
            .WithName("ChangeBookingStatus")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy => policy.RequireRole("Admin", "VenueOwner"));



            group.MapGet("/{id:guid}", async (Guid id, ISender mediator) =>
            {
                var result = await mediator.Send(new GetBookingByIdDto(id));
                return Results.Ok(result);
            })
            .WithName("GetBookingById")
            .Produces<ResponseBookingDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();

           
            group.MapGet("/", async (ISender mediator) =>
            {
                var result = await mediator.Send(new GetAllBookingsDto());
                return Results.Ok(result);
            })
            .WithName("GetAllBookings")
            .Produces<List<ResponseBookingDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(policy => policy.RequireRole("Admin"));


            group.MapGet("/my-bookings", async (ISender mediator) =>
            {
                var result = await mediator.Send(new GetMyBookingsQuery());
                return Results.Ok(result);
            })
            .WithName("GetMyBookings")
            .Produces<List<ResponseBookingDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();

            
            group.MapGet("/venue/{venueId:guid}", async (Guid venueId, ISender mediator) =>
            {
                var result = await mediator.Send(new GetVenueBookingsQuery(venueId));
                return Results.Ok(result);
            })
            .WithName("GetVenueBookings")
            .Produces<List<ResponseBookingDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();
        }
    }
}

//{
//    "hallId": "C8592BB2-2F05-4D58-B6F4-5FCAB5353C8B",
//  "startTime": "2026-08-01T18:00:00Z",
//  "endTime": "2026-08-01T22:00:00Z",
//  "services": [
//    {
//        "serviceId": "D3582465-4F9D-4A51-B37B-01AD0864DAF9",
//      "quantity": 2
//    },
//    {
//        "serviceId": "09BA1143-B107-4E48-9C6D-0692BC480D5C",
//      "quantity": 1
//    }
//  ]
//}