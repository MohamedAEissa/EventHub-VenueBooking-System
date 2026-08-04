using EventHub.Application.Features.Venues.Dtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Venues.Validators
{
    public class CreateVenueDtoValidator:AbstractValidator<CreateVenueDto>
    {
        public CreateVenueDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is Required")
                .MaximumLength(150).WithMessage("Name Must be less than 150 char");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is Required");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Detailed Address is Required");
        }
    }
}
