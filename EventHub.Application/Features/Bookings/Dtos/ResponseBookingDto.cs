using EventHub.Application.Features.Services.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Bookings.Dtos
{
    public class ResponseBookingDto
    {
        public Guid Id { get; set; }
        public Guid HallId { get; set; }
        public string HallName { get; set; } = string.Empty;
        public string VenueName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<ResponseBookingServicesDto> Services { get; set; } = new();

    }
}
