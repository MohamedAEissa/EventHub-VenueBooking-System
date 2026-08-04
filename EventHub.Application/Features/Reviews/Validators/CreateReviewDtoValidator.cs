using EventHub.Application.Features.Reviews.Dtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Reviews.Validators
{
    public class CreateReviewDtoValidator:AbstractValidator<CreateReviewDto>
    {
        public CreateReviewDtoValidator()
        {
            RuleFor(x => x.Rate)
            .InclusiveBetween(1, 5).WithMessage("Rate Must be Between 1-5");

            RuleFor(x => x.Comment)
                .NotEmpty().WithMessage("Comment requierd")
                .MaximumLength(1000).WithMessage("Comment at most 1000 char");
        }
    }
}
