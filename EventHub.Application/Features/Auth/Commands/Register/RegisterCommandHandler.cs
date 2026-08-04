using AutoMapper;
using EventHub.Application.Common.InterFaces;
using EventHub.Application.Features.Auth.Dtos;
using EventHub.Domain.Entities;
using EventHub.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Auth.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterDto, AuthResponseDto>
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly IMapper mapper;
        private readonly ITokenService tokenService;
        private readonly JwtSettings jwtSettings;

        public RegisterCommandHandler(
                UserManager<User> userManager,
                RoleManager<ApplicationRole> roleManager,
                IMapper mapper,
                ITokenService tokenService,
                IOptions<JwtSettings> jwtSettings) 
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.mapper = mapper;
            this.tokenService = tokenService;
            this.jwtSettings = jwtSettings.Value; 
        }
        public async Task<AuthResponseDto> Handle(RegisterDto request, CancellationToken cancellationToken)
        {
            var existingUser= await userManager.FindByEmailAsync(request.Email);
            if (existingUser != null) 
            {
                throw new Exception("Email is Exist");            
            }

            var user = mapper.Map<User>(request);


            var result =await userManager.CreateAsync(user,request.Password);

            if (!result.Succeeded)
            {
                var error = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Error While creating Account: {error}");
            }

            if (!String.IsNullOrEmpty(request.Role))
            {
                if (!await roleManager.RoleExistsAsync(request.Role))
                {
                    await roleManager.CreateAsync(new ApplicationRole(request.Role));
                }
                await userManager.AddToRoleAsync(user,request.Role);
            }
            var userRoles = (await userManager.GetRolesAsync(user)).ToList();
            
            var token = tokenService.CreateToken(user, userRoles);
            var refreshToken = tokenService.GenerateRefreshToken();
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(jwtSettings.RefreshTokenDurationInDays);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = refreshTokenExpiry;
            await userManager.UpdateAsync(user);

            var response = mapper.Map<AuthResponseDto>(user);
            response.Role = userRoles.FirstOrDefault();
            response.Token = token;
            response.RefreshToken=refreshToken;
            response.RefreshTokenExpiryDate = refreshTokenExpiry;
            
            return response;
        }
    }
}
