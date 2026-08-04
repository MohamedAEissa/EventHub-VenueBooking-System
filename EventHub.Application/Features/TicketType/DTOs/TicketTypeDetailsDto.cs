using EventHub.Application.Features.Events.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.TicketType.DTOs
{
    public class TicketTypeDetailsDto : ResponseTicketTypeDto
    {
        public string EventTitle { get; set; } = string.Empty;
        public string HallName { get; set; } = string.Empty;
        public string VenueName { get; set; } = string.Empty;
    }
}
