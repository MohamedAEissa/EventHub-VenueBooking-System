using EventHub.Application.Features.Halls.Dtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Halls.Validators
{
    public class CreateHallDtoValidator:AbstractValidator<CreateHallDto>
    {
        public CreateHallDtoValidator()
        {
            RuleFor(x=>x.Name)
                .NotEmpty().WithMessage("You Must Enter Name");

            RuleFor(x => x.Capacity)
                .GreaterThan(0).WithMessage("Hall Capacity Must Be Greater Than 0");

            RuleFor(x => x.PricePerHour)
                .GreaterThan(0).WithMessage("Price Must Be Greater Than 0");

        }


    }
}
