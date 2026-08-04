using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Services.Dtos
{
    public class CreateBookingServiceDto : IRequest<ResponseBookingServicesDto>
    {
        public Guid ServiceId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
