using EventHub.Application.Features.Services.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Bookings.Dtos
{
    public class UpdateBookingDto : IRequest<ResponseBookingDto>
    {
        public Guid BookingId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<CreateBookingServiceDto> Services { get; set; } = new();
    }
}
