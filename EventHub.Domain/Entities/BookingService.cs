using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Domain.Entities
{
    public class BookingService
    {
        public Guid BookingId { get; set; }
        public Booking Booking { get; set; } = null!;

        public Guid ServiceId { get; set; }
        public Service Service { get; set; } = null!;

        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
