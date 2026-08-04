using EventHub.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Bookings.Dtos
{
    public class ChangeBookingStatusDto:IRequest<bool>
    {
        public Guid BookingId { get; set; }
        public BookingStatus NewStatus { get; set; }
    }
}
