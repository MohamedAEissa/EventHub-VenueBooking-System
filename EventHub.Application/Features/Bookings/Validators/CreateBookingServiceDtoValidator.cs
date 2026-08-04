using EventHub.Application.Features.Services.Dtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Bookings.Validators
{
    public class CreateBookingServiceDtoValidator:AbstractValidator<CreateBookingServiceDto>
    {
        public CreateBookingServiceDtoValidator()
        {
            RuleFor(s => s.ServiceId)
            .NotEmpty().WithMessage("You must enter ServiceId");

            RuleFor(x=>x.Quantity)
                .NotEmpty().WithMessage("You must enter quantity")
                .GreaterThan(0).WithMessage("your quantity must be greater than 0");

        }



    }
}
