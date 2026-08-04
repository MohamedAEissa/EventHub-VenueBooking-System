using EventHub.Application.Features.TicketType.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.TicketType.Validators
{
    public class UpdateTicketTypeValidator:AbstractValidator<UpdateTicketTypeDto>
    {
        public UpdateTicketTypeValidator()
        {
            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Ticket type name is required.")
                .MaximumLength(50).WithMessage("Ticket type name must not exceed 50 characters.");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price cannot be negative.");

            RuleFor(x => x.TotalQuantity)
                .GreaterThan(0).WithMessage("Total quantity must be at least 1.");
        }
    }
}
