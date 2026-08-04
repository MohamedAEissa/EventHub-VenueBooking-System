using EventHub.Application.Features.Auth.Dtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Auth.Validators
{
    public class RevokeTokenDtoValidator:AbstractValidator<RevokeTokenDto>
    {
        public RevokeTokenDtoValidator()
        {
            RuleFor(x=>x.RefreshToken)
                .NotEmpty().WithMessage("Refresh Token is required for logout.");
        }

    }
}
