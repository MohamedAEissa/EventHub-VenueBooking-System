using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Services.Dtos
{
    public class ResponseBookingServicesDto
    {
        public Guid ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; } 
        public int Quantity { get; set; }      
        public decimal TotalPrice => UnitPrice * Quantity; 
    }
}
