using EventHub.Application.Features.Auth.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Events.Dtos
{
    public class CreateEventDto: IRequest<ResponseEventDto>
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public Guid HallId { get; set; }
        public List<CreateTicketTypeSubDto> TicketTypes { get; set; } = new();

    }
}
