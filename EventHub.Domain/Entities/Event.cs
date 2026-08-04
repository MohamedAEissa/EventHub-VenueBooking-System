using EventHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Domain.Entities
{
    public class Event : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public Guid HallId { get; set; }
        public Hall Hall { get; set; } = null!;
        public ICollection<TicketTier> Tickets { get; set; } = new List<TicketTier>();//ticket tier
    }
}
