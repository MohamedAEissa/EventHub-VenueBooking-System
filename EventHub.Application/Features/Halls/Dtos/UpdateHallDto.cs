using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Halls.Dtos
{
    public class UpdateHallDto : IRequest<ResponseHallDto>
    {
        public Guid VenueId { get; set; }
        public Guid HallId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public decimal PricePerHour { get; set; }
    }
}
