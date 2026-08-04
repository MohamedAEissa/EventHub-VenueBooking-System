using EventHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Domain.Entities
{
    public class TicketTier : BaseEntity
    {
        public string Type { get; set; } = string.Empty; 
        public decimal Price { get; set; }
        public int TotalQuantity { get; set; }
        public int AvailableQuantity { get; set; }
        public Guid EventId { get; set; }
        public Event Event { get; set; } = null!;
        public ICollection<UserTicket> UserTickets { get; set; } = new List<UserTicket>();
    }
}
