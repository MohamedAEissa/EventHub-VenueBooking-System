using EventHub.Application.Features.Auth.Dtos;
using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using System.Threading.Tasks;

namespace EventHub.API.Endpoints.Auth
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndPoint(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/auth")
                                        .WithTags("Auth");

            group.MapPost("register", async (RegisterDto dto, ISender mediator) =>
            {
                var result = await mediator.Send(dto);
                return Results.Ok(result);
            })
            .WithName("RegisterUser")
            .Produces<AuthResponseDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);



            group.MapPost("login", async (LoginDto dto, ISender mediator) =>
            { 
                var result=await mediator.Send(dto);
                return Results.Ok(result);
            })
            .WithName("LoginUser")
            .Produces<AuthResponseDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);


            group.MapPost("refresh-token", async (RefreshTokenDto dto, ISender mediator) =>
            {
                var result = await mediator.Send(dto);
                return Results.Ok(result);
            })
            .WithName("RefreshToken")
            .Produces<AuthResponseDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);

            group.MapPost("logout", async (RevokeTokenDto dto , ISender mediator)=>
            {
                var result = await mediator.Send(dto);

                return Results.Ok(new {message= "Logged out successfully" });
            }
            ).WithName("logoutUser")
            .Produces<AuthResponseDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        }

    }
}
