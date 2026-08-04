using EventHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Domain.Entities
{
    public class Venue:BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        
        public Guid OwnerId { get; set; }
        public User Owner { get; set; }

        public ICollection<Hall> Halls { get; set; } = new List<Hall>();
        public ICollection<Service> Services { get; set; } = new List<Service>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();

    }
}
