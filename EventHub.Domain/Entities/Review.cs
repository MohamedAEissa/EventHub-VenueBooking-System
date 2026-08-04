using EventHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Domain.Entities
{
    public class Review : BaseEntity
    {
        public int Rate { get; set; } 
        public string Comment { get; set; } = string.Empty;

        public Guid VenueId { get; set; }
        public Venue Venue { get; set; } = null!;

        public Guid ClientId { get; set; }
        public User Client { get; set; } = null!;
    }
}
