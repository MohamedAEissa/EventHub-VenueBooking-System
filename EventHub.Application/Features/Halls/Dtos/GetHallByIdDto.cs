using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Halls.Dtos
{
    public class GetHallByIdDto : IRequest<ResponseHallDto>
    {
        public GetHallByIdDto(Guid venueId, Guid hallId)
        {
            VenueId = venueId;
            HallId = hallId;

        }
        public Guid HallId { get; set; }
        public Guid VenueId { get; set; }
        
    }
}
