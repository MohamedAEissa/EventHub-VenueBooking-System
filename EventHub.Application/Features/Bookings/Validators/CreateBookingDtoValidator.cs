using EventHub.Application.Features.Bookings.Dtos;
using EventHub.Application.Features.Services.Validators;

using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Bookings.Validators
{
    public class CreateBookingDtoValidator:AbstractValidator<CreateBookingDto>
    {
        public CreateBookingDtoValidator()
        {
            RuleFor(x => x.StartTime)
                .NotNull().WithMessage("You must enter End Time")
                .GreaterThan(DateTime.UtcNow).LessThan(x=>x.EndTime).WithMessage("You must enter Start Time In future and Must Be Less Than End Time ");

            RuleFor(x => x.EndTime)
               .NotNull().WithMessage("You must enter End Time")
               .GreaterThan(DateTime.UtcNow).WithMessage("You must enter Start Time In future");

            RuleForEach(x => x.Services)
                .SetValidator(new CreateBookingServiceDtoValidator());
        }
    }
}
