using EventHub.Application.Features.Auth.Dtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Auth.Validators
{
    public class RegisterDtoValidator:AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x=>x.FullName)
                .NotEmpty().WithMessage("You Must Enter FullName")
                .MaximumLength(100).WithMessage("Fullname Must Be Lower than 100 Char");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("You Must Enter Email")
                .EmailAddress().WithMessage("You Must Enter Valid Email");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("You Must Enter Password")
                .MinimumLength(6).WithMessage("Password Must Be At least 6 char")
                .Matches("[A-Z]").WithMessage("Password Must Be contain at least 1 capital letter")
                .Matches("[0-9]").WithMessage("Password Must Be contain at least 1 Number");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("You Must Enter Role")
                .Must(role => role == "Client" || role == "VenueOwner" || role == "Admin")
                .WithMessage("Rule Must Be One Of This [Client,VenueOwner,Admin");
        }
    }
}
