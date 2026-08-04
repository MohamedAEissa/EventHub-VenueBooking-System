using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Auth.Dtos
{
    public class LoginDto : IRequest<AuthResponseDto>
    {
        public string Email { get; set; }
        public string Password { get; set; }

    }
}   
