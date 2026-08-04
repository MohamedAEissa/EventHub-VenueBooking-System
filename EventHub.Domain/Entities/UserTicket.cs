using EventHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Domain.Entities
{
    public class UserTicket : BaseEntity
    {
        public string TicketCode { get; set; } = string.Empty;
        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
        public bool IsUsed { get; set; } = false;

        public Guid TicketId { get; set; }
        public TicketTier Ticket { get; set; } = null!;

        public Guid ClientId { get; set; }
        public User Client { get; set; } = null!;
    }
}
