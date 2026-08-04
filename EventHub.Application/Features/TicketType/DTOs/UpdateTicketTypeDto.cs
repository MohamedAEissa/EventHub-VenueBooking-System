using EventHub.Application.Features.Events.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.TicketType.DTOs
{
    public class UpdateTicketTypeDto :IRequest<ResponseTicketTypeDto>
    {
        public Guid TicketTypeId { get; set; }
        public string Type { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int TotalQuantity { get; set; }
    }
}
