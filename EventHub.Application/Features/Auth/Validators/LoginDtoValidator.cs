using EventHub.Application.Features.Auth.Dtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Auth.Validators
{
    public class LoginDtoValidator:AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x=>x.Email)
                .NotEmpty().WithMessage("You Must Enter Email")
                .EmailAddress().WithMessage("You Must Enter Valid Email");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("You must enter Password");
        }
    }
}
