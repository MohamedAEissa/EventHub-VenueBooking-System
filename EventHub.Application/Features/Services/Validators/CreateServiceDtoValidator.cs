using EventHub.Application.Features.Services.Dtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Services.Validators
{
    public class CreateServiceDtoValidator:AbstractValidator<CreateServiceDto>
    {
        public CreateServiceDtoValidator()
        {
            RuleFor(x => x.Name)
             .NotEmpty().WithMessage("Name is required")
             .MaximumLength(100).WithMessage("name must be less than 100 char");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description must be less than 500 char");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price must be 0 or more");

        }
    }
}
