using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Events.Dtos
{
    public class ResponseTicketTypeDto
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public string Type { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int TotalQuantity { get; set; }
        public int AvailableQuantity { get; set; }
    }
}
