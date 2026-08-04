using AutoMapper;
using EventHub.Application.Common.InterFaces;
using EventHub.Application.Features.Auth.Dtos;
using EventHub.Domain.Entities;
using EventHub.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens.Experimental;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginDto, AuthResponseDto>
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> singInManager;
        private readonly IMapper mapper;
        private readonly ITokenService tokenService;
        private readonly JwtSettings jwtSettings;

        public LoginCommandHandler(
            
            UserManager<User> userManager,
            SignInManager<User> singInManager,
            IMapper mapper,
            ITokenService tokenService,
            IOptions<JwtSettings> jwtSettings
            )
        {
            this.userManager = userManager;
            this.singInManager = singInManager;
            this.mapper = mapper;
            this.tokenService = tokenService;
            this.jwtSettings = jwtSettings.Value;
        }
        public async Task<AuthResponseDto> Handle(LoginDto request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                throw new Exception("Invalid Email or Password");
            }

            var result = await singInManager.CheckPasswordSignInAsync(user, request.Password,lockoutOnFailure:false);
            if (!result.Succeeded)
            {
                throw new Exception("Invalid Email or Password");
            }

            var userRoles = (await userManager.GetRolesAsync(user)).ToList();

            var token = tokenService.CreateToken(user,userRoles);
            var refreshToken = tokenService.GenerateRefreshToken();
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(jwtSettings.RefreshTokenDurationInDays);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = refreshTokenExpiry;
            await userManager.UpdateAsync(user);

            var response = mapper.Map<AuthResponseDto>(user);
            response.Role = userRoles.FirstOrDefault();
            response.Token = token;
            response.RefreshToken = refreshToken;
            response.RefreshTokenExpiryDate = refreshTokenExpiry;

            return response;

        }
    }
}
