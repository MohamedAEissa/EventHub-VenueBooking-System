using EventHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Domain.Entities
{
    public class Hall:BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public decimal PricePerHour { get; set; }
        public bool IsActive { get; set; } = true;

        public Guid VenueId { get; set; }
        public Venue Venue { get; set; } = null!;

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<Event> Events { get; set; } = new List<Event>();

    }

}
