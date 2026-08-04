using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Application.Features.Halls.Dtos
{
    public class GetAllHallsByVenueIdDto : IRequest<IEnumerable<ResponseHallDto>>
    {
        public Guid VenueId { get; set; }

        public GetAllHallsByVenueIdDto(Guid venueId)
        {
            VenueId = venueId;
        }
    }
}
