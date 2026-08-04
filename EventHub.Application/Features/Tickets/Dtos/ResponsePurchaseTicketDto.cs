using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Tickets.Dtos
{
    public class ResponsePurchaseTicketDto
    {
        public Guid EventId { get; set; }
        public string EventTitle { get; set; } = string.Empty;
        public string TicketType { get; set; } = string.Empty;
        public int QuantityPurchased { get; set; }
        public decimal TotalPrice { get; set; }
        public List<Guid> TicketIds { get; set; } = new();
    }
}
