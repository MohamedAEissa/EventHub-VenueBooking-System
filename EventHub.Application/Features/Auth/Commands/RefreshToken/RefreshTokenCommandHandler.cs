using EventHub.Application.Common.InterFaces;
using EventHub.Application.Features.Auth.Dtos;
using EventHub.Domain.Entities;
using EventHub.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenDto, AuthResponseDto>
    {
        private readonly UserManager<User> userManager;
        private readonly ITokenService tokenService;
        private readonly JwtSettings jwtSetting;

        public RefreshTokenCommandHandler(
            UserManager<User> userManager,
            ITokenService tokenService,
            IOptions<JwtSettings> jwtSetting
            )
        {
            this.userManager = userManager;
            this.tokenService = tokenService;
            this.jwtSetting = jwtSetting.Value;
        }

        public async Task<AuthResponseDto> Handle(RefreshTokenDto request, CancellationToken cancellationToken)
        {
            var principal = tokenService.GetPrincipalFromExpiredToken(request.Token);
            if (principal == null)
            {
                throw new Exception("Invalid Access Token or Refresh Token");
            }

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                throw new Exception("Invalid Access Token");
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null||user.RefreshToken!=request.RefreshToken||user.RefreshTokenExpiryTime<=DateTime.UtcNow) 
            {
                throw new Exception("Invalid or Expired Refresh Token");
            }

            var userRoles = (await userManager.GetRolesAsync(user)).ToList();
            var newAccessToken = tokenService.CreateToken(user,userRoles);
            var newRefreshToken = tokenService.GenerateRefreshToken();
            var newRefreshTokenExpiry= DateTime.UtcNow.AddDays(jwtSetting.RefreshTokenDurationInDays);

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = newRefreshTokenExpiry;
            await userManager.UpdateAsync(user);

            return new AuthResponseDto
            {
                UserId = user.Id,
                Email = user.Email!,
                FullName = user.FullName,
                Role = userRoles.FirstOrDefault(),
                Token = newAccessToken,
                RefreshToken = newRefreshToken,
                RefreshTokenExpiryDate = newRefreshTokenExpiry
            };
        }
    }
}
