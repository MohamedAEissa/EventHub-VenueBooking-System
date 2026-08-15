using EventHub.API.Endpoints.Auth;
using EventHub.API.Endpoints.BookingEndpoints;
using EventHub.API.Endpoints.Events;
using EventHub.API.Endpoints.Halls;
using EventHub.API.Endpoints.Review;
using EventHub.API.Endpoints.Services;
using EventHub.API.Endpoints.Ticket;
using EventHub.API.Endpoints.TicketType;
using EventHub.API.Endpoints.Venues;
using EventHub.API.Extensions;
using EventHub.Application;
using EventHub.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

try
{
    Log.Information("Starting EventHub Web API...");
    // Register Services
    builder.Services
		.AddInfrastructureServices(builder.Configuration)
		.AddApplicationServices()
		.AddApiServices(builder.Configuration, builder.Host);

	var app = builder.Build();

	// Configure Middleware Pipeline
	app.UseApiMiddelwares();

	// Map Endpoints
	app.MapAuthEndPoint();
	app.MapVenueEndpoints();
	app.MapHallEndpoints();
	app.MapServiceEndpoints();
	app.MapBookingEndpoints();
	app.MapEventsEndpoints();
	app.MapTicketTypeEndpoints();
	app.MapTicketEndpoints();
	app.MapReviewEndpoints();
	app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "EventHub Host terminated unexpectedly!");
}
finally
{
    Log.CloseAndFlush(); //save logs
}

//add chaanges to check  commit