using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Events.Dtos
{
    public class ResponseEventDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public Guid HallId { get; set; }
        public string HallName { get; set; } = string.Empty;
        public string VenueName { get; set; } = string.Empty;
        public List<ResponseTicketTypeDto> TicketTypes { get; set; } = new();
    }
}
