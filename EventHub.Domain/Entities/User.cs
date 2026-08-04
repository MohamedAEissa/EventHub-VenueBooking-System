using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Domain.Entities
{
    public  class User :IdentityUser<Guid>
    {
        public string FullName { get; set; }=string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedAt { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        public ICollection<Venue> Venues { get; set; } = new List<Venue>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<TicketTier> Tickets { get; set; } = new List<TicketTier>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
