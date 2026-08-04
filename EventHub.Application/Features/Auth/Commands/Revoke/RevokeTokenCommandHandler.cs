using EventHub.Application.Features.Auth.Dtos;
using EventHub.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Auth.Commands.Revoke
{
    public class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenDto,bool>
    {
        private readonly UserManager<User> userManager;

        public RevokeTokenCommandHandler(UserManager<User> userManager)
        {
            
            this.userManager = userManager;
        }
        public async Task<bool> Handle(RevokeTokenDto request, CancellationToken cancellationToken)
        {
            if (String.IsNullOrEmpty(request.RefreshToken))
                throw new Exception("Token is required");

            var user = await userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken);

            if (user == null)
                throw new Exception("Invalid Token");

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
                throw new Exception("Failed to revoke token");

            return true;

       
    }
}
    }
