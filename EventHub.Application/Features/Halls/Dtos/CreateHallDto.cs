using EventHub.Application.Features.Auth.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Halls.Dtos
{
    public class CreateHallDto: IRequest<ResponseHallDto>
    {
        public string Name { get; set; }= string.Empty;
        public int Capacity { get; set; }
        public decimal PricePerHour { get; set; }
        public Guid VenueId { get; set; }

    }
}
