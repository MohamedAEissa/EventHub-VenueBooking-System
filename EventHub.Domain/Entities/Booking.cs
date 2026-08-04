using EventHub.Domain.Common;
using EventHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Domain.Entities
{
    public class Booking:BaseEntity
    {
        //user who booked
        public Guid ClientId { get; set; }
        public User Client { get; set; } = null!;

        //hall that booked
        public Guid HallId { get; set; }
        public Hall Hall { get; set; } = null!;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalPrice { get; set; }
        public BookingStatus Status { get; set; } = BookingStatus.Pending;
        public ICollection<BookingService> BookingServices { get; set; } = new List<BookingService>();
    }
}
