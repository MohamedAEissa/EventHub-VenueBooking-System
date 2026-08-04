using EventHub.Application.Features.Auth.Dtos;
using EventHub.Application.Features.Bookings.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Services.Dtos
{
    public class CreateServiceDto: IRequest<ResponseServiceDto>
    {
        public Guid VenueId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
