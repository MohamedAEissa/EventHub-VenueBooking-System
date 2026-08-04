using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Auth.Dtos
{
    public class RevokeTokenDto:IRequest<bool>
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
