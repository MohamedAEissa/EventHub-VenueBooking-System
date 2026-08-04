using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Tickets.Dtos
{
    public class ResponseUserTicketDto
    {
        public Guid TicketId { get; set; }
        public string TicketCode { get; set; } = string.Empty;
        public DateTime PurchaseDate { get; set; }
        public bool IsUsed { get; set; }
        public string EventTitle { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string TicketType { get; set; } = string.Empty;
        public decimal PricePaid { get; set; }
    }
}
