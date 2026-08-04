using EventHub.Application.Features.Tickets.Dtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Tickets.Validators
{
    public class PurchaseTicketValidator : AbstractValidator<PurchaseTicketDto>
    {
        public PurchaseTicketValidator()
        {
            RuleFor(x => x.TicketTypeId)
                .NotEmpty().WithMessage("TicketTypeId is required.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be at least 1.")
                .LessThanOrEqualTo(10).WithMessage("You cannot purchase more than 10 tickets at once.");
        }
    }
}
