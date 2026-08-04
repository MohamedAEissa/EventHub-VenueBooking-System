using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Auth.Dtos
{
    public class RefreshTokenDto: IRequest<AuthResponseDto>
    {
        public string Token { get; set; } = string.Empty;     
        public string RefreshToken { get; set; } = string.Empty;
    }
}
